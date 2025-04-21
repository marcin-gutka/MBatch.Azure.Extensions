using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    public static partial class BatchClientExtensions
    {
        private const int TIMEOUT = 5; // 5 min

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

        public static IEnumerable<CloudJob?> GetPoolJobsAsync(this BatchClient batchClient, string poolId)
        {
            if (string.IsNullOrEmpty(poolId))
                throw new ArgumentNullException(nameof(poolId));

            var jobs = batchClient.JobOperations.ListJobs();

            foreach (var job in jobs)
            {
                if (job.PoolInformation.PoolId == poolId)
                {
                    yield return job;
                }
            }
        }

        public static async Task DeletePoolAsync(this BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)
        {
            var pool = await batchClient.PoolOperations.GetPoolAsync(poolId, cancellationToken: cancellationToken);

            await pool.DeleteAsync(cancellationToken: cancellationToken);
        }

        public static async Task RebootNodesAsync(this BatchClient batchClient, string poolId, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)
        {
            var pool = await batchClient.PoolOperations.GetPoolAsync(poolId, cancellationToken: cancellationToken);

            await batchClient.RebootNodesAsync(pool, computeNodeRebootOption, cancellationToken);
        }

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
                if (node.State != ComputeNodeState.Rebooting)
                    taskList.Add(node.RebootAsync(computeNodeRebootOption));
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
