using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    public static partial class BatchClientExtensions
    {
        private const int TIMEOUT = 5; // 5 min

        /// <summary>
        /// Checks if pool exist in Batch Account.
        /// This method calls <see cref="PoolOperations.GetPoolAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="poolId">Pool Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when a pool exists, otherwise <see langword="false"/>.</returns>
        /// <exception cref="BatchException">Passing through except when a pool is not found.</exception>
        public static async Task<bool> DoesPoolExistAsync(this BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)
        {
            try
            {
                await batchClient.PoolOperations.GetPoolAsync(poolId, cancellationToken: cancellationToken);
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.PoolNotFound)
                    return false;

                throw;
            }

            return true;
        }

        /// <summary>
        /// Gets all jobs for a pool in Batch Account.
        /// This method calls <see cref="JobOperations.ListJobs(DetailLevel, IEnumerable{BatchClientBehavior})"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="poolId">Pool Id.</param>
        /// <returns>Collection of <see cref="CloudJob"/>.</returns>
        /// <exception cref="BatchException">Passing through except when a pool is not found.</exception>
        public static IEnumerable<CloudJob> GetPoolJobs(this BatchClient batchClient, string poolId)
        {
            if (string.IsNullOrEmpty(poolId))
                throw new ArgumentNullException(nameof(poolId));

            var jobs = batchClient.JobOperations.ListJobs();

            foreach (var job in jobs)
            {
                if (job is not null && job.PoolInformation.PoolId == poolId)
                {
                    yield return job;
                }
            }
        }

        /// <summary>
        /// Deletes a pool in Batch Account.
        /// This method calls <see cref="PoolOperations.GetPoolAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="poolId">Pool Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when a pool is deleted, otherwise <see langword="false"/>.</returns>
        /// <exception cref="BatchException">Passing through except when a pool is not found.</exception>
        public static async Task<bool> DeletePoolIfExistsAsync(this BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)
        {
            try
            {
                var pool = await batchClient.PoolOperations.GetPoolAsync(poolId, cancellationToken: cancellationToken);

                await pool.DeleteAsync(cancellationToken: cancellationToken);

                return true;
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.PoolNotFound)
                    return false;

                throw;
            }
        }

        /// <summary>
        /// Reboots nodes within a pool in Batch Account.
        /// This method calls following methods: <see cref="PoolOperations.GetPoolAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>, 
        /// <see cref="CloudPool.StopResizeAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="PoolOperations.ListComputeNodes(string, DetailLevel, IEnumerable{BatchClientBehavior})"/>,
        /// <see cref="ComputeNode.RebootAsync(ComputeNodeRebootOption?, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="poolId">Pool Id.</param>
        /// <param name="computeNodeRebootOption">Action to execute when node is rebooted.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="BatchException">Passing through except when a pool is not found.</exception>
        /// <remarks>Waits up to 5 minutes for pool to be in steady state if needed.</remarks>
        public static async Task RebootNodesAsync(this BatchClient batchClient, string poolId, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)
        {
            var pool = await batchClient.PoolOperations.GetPoolAsync(poolId, cancellationToken: cancellationToken);

            await batchClient.RebootNodesAsync(pool, computeNodeRebootOption, cancellationToken);
        }

        /// <summary>
        /// Reboots nodes within a pool in Batch Account.
        /// This method calls following methods: <see cref="CloudPool.StopResizeAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="PoolOperations.ListComputeNodes(string, DetailLevel, IEnumerable{BatchClientBehavior})"/>,
        /// <see cref="ComputeNode.RebootAsync(ComputeNodeRebootOption?, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="pool">Pool object.</param>
        /// <param name="computeNodeRebootOption">Action to execute when node is rebooted.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="BatchException">Passing through except when a pool is not found.</exception>
        /// <remarks>Waits up to 5 minutes for pool to be in steady state if needed.</remarks>
        public static async Task RebootNodesAsync(this BatchClient batchClient, CloudPool pool, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)
        {
            if (pool.AllocationState == AllocationState.Resizing)
            {
                await pool.StopResizeAsync(cancellationToken: cancellationToken);
                await WaitUntilPoolIsSteadyAsync(batchClient, pool.Id, cancellationToken);
            }

            // create service to handle nodes and spot nodes
            var nodes = batchClient.PoolOperations.ListComputeNodes(pool.Id);

            var taskList = new List<Task>();

            foreach (var node in nodes)
            {
                if (node is not null && node.State != ComputeNodeState.Rebooting)
                    taskList.Add(node.RebootAsync(computeNodeRebootOption, cancellationToken: cancellationToken));
            }

            await Task.WhenAll(taskList);
        }

        private static async Task WaitUntilPoolIsSteadyAsync(BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)
        {
            var poolState = AllocationState.Resizing;

            var iteration = 0;

            while (!cancellationToken.IsCancellationRequested && poolState != AllocationState.Steady)
            {
                if (iteration >= TIMEOUT * 60)
                    throw new TimeoutException($"Timeout occurred when stopping resizing for pool: {poolId}.");

                var pool = await batchClient.PoolOperations.GetPoolAsync(poolId, cancellationToken: cancellationToken);

                poolState = pool.AllocationState ?? poolState;

                await Task.Delay(1000, cancellationToken);
                iteration++;
            }
        }
    }
}
