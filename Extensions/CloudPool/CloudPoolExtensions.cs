using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for extensions methods for <see cref="CloudPool"/>.
    /// </summary>
    public static class CloudPoolExtensions
    {
        /// <summary>
        /// Sets number of nodes for pool.
        /// This method calls following methods: <see cref="CloudPool.RefreshAsync(DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudPool.ResizeAsync(int?, int?, TimeSpan?, ComputeNodeDeallocationOption?, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudPool.RemoveFromPool(IEnumerable{ComputeNode}, ComputeNodeDeallocationOption?, TimeSpan?, IEnumerable{BatchClientBehavior})"/>.
        /// </summary>
        /// <param name="pool">CloudPool object.</param>
        /// <param name="targetNodeCount">Number of nodes to set.</param>
        /// <param name="computeNodeDeallocationOption">Action to be perform on task when a node is removed.</param>
        /// <param name="logger">Optional: for logging.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>When removing nodes, the custom comparer is used to determine which nodes are most efficient to be removed. AutoScale has to be disable. Low priority nodes are not currently supported by this extension.</remarks>
        public static async Task SetTargetNodesCountAsync(this CloudPool pool, int targetNodeCount, ComputeNodeDeallocationOption computeNodeDeallocationOption, ILogger? logger, CancellationToken cancellationToken = default)
        {
            await pool.RefreshAsync(cancellationToken: cancellationToken);

            if (pool.AutoScaleEnabled is not null && pool.AutoScaleEnabled.Value)
            {
                logger?.LogWarning("Cannot resize pool: '{PoolId}' as AutoScale is enabled", pool.Id);
                return;
            }

            if (pool.AllocationState != AllocationState.Steady)
            {
                logger?.LogWarning("Cannot resize pool: '{PoolId}' as its state is not steady", pool.Id);
                return;
            }

            if (pool.TargetDedicatedComputeNodes < targetNodeCount)
            {
                logger?.LogInformation("New target node count: {TargetNodeCount}, previous: {TargetDedicatedComputeNodes} - adding nodes", targetNodeCount, pool.TargetDedicatedComputeNodes);

                await pool.ResizeAsync(targetNodeCount, deallocationOption: computeNodeDeallocationOption, cancellationToken: cancellationToken);
            }

            // use this instead of ResizeAsync when removing nodes to choose exactly which nodes are removed.
            if (pool.TargetDedicatedComputeNodes > targetNodeCount)
            {
                logger?.LogInformation("New target node count: {TargetNodeCount}, previous: {TargetDedicatedComputeNodes} - removing nodes", targetNodeCount, pool.TargetDedicatedComputeNodes);

                var nodesToDelete = NodesToDelete(pool, targetNodeCount);

                await pool.RemoveFromPoolAsync(nodesToDelete, deallocationOption: computeNodeDeallocationOption, cancellationToken: cancellationToken);
            }
        }

        private static IEnumerable<ComputeNode> NodesToDelete(CloudPool pool, int targetNodeCount)
        {
            var nodes = pool.ListComputeNodes().ToList();

            var nodesCount = pool.TargetDedicatedComputeNodes;

            if (nodesCount is null || nodesCount <= targetNodeCount)
                return [];

            var orderedNodes = nodes.OrderBy(x => x.State, new NodeStateComparerForRemoving());

            return orderedNodes.Take(nodesCount.Value - targetNodeCount);
        }

        /// <summary>
        /// Recovers unhealty nodes in a pool.
        /// This method calls following methods: <see cref="CloudPool.RefreshAsync(DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudPool.ListComputeNodes(DetailLevel, IEnumerable{BatchClientBehavior})"/>,
        /// <see cref="ComputeNode.EnableSchedulingAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="ComputeNode.RebootAsync(ComputeNodeRebootOption?, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="ComputeNode.RemoveFromPoolAsync(ComputeNodeDeallocationOption?, TimeSpan?, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// </summary>
        /// <param name="pool">CloudPool object.</param>
        /// <param name="logger">Optional: for logging.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="int"/>: Number of unhealthy nodes.</returns>
        public static async Task<int> RecoverUnhealthyNodesAsync(this CloudPool pool, ILogger? logger, CancellationToken cancellationToken = default)
        {
            await pool.RefreshAsync(cancellationToken: cancellationToken);

            var unhealthyNodes = pool.ListComputeNodes(new ODATADetailLevel()
            {
                FilterClause = GetUnhealthyNodesFilter()
            });

            var taskList = new List<Task>();

            logger?.LogInformation("Checking unhealthy nodes for pool: '{PoolId}'.", pool.Id);

            foreach (var node in unhealthyNodes)
            {
                switch (node.State)
                {
                    case ComputeNodeState.Offline or ComputeNodeState.Deallocated:
                        taskList.Add(node.EnableSchedulingAsync(cancellationToken: cancellationToken));
                        logger?.LogInformation("Node '{NodeId}' was '{State}' in pool '{PoolId}'. Enabling scheduling was activated.", node.Id, node.State.ToString(), pool.Id);
                        break;
                    case ComputeNodeState.Unusable:
                        taskList.Add(node.RebootAsync(rebootOption: ComputeNodeRebootOption.Requeue, cancellationToken: cancellationToken));
                        logger?.LogInformation("Node '{NodeId}' was '{State}' in pool '{PoolId}'. Rebooting was triggered.", node.Id, node.State.ToString(), pool.Id);
                        break;
                    case ComputeNodeState.Unknown:
                        if (pool.AllocationState != AllocationState.Resizing)
                        {
                            taskList.Add(node.RemoveFromPoolAsync(deallocationOption: ComputeNodeDeallocationOption.Requeue, cancellationToken: cancellationToken));
                            logger?.LogInformation("Node '{NodeId}' was '{State}' in pool '{PoolId}'. Node will be removed.", node.Id, node.State.ToString(), pool.Id);
                        }
                        break;
                }
            }

            var numberOfUnhealthyNodes = taskList.Count;

            if (numberOfUnhealthyNodes > 0)
                logger?.LogInformation("Found {TaskListCount} unhealthy nodes in pool '{PoolId}'.", numberOfUnhealthyNodes, pool.Id);

            await Task.WhenAll(taskList);

            return numberOfUnhealthyNodes;
        }

        private static string GetUnhealthyNodesFilter() =>
            "state eq 'offline' or state eq 'deallocated' or state eq 'unusable' or state eq 'unknown'";

        #region Node State Comparer
        private class NodeStateComparerForRemoving : IComparer<ComputeNodeState?>
        {
            public int Compare(ComputeNodeState? x, ComputeNodeState? y)
            {
                if (x == y)
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                var orderX = GetOrder(x.Value);
                var orderY = GetOrder(y.Value);

                if (orderX == orderY)
                    return 0;

                if (orderX < orderY)
                    return 1;

                return -1;
            }

            private static int GetOrder(ComputeNodeState x) =>
                x switch
                {
                    ComputeNodeState.Running => 1,
                    ComputeNodeState.Idle => 2,
                    ComputeNodeState.WaitingForStartTask => 3,
                    ComputeNodeState.Starting => 4,
                    ComputeNodeState.Rebooting => 5,
                    ComputeNodeState.Creating => 6,
                    // any other case is treated in the same way
                    _ => 20
                };
        }
        #endregion
    }
}
