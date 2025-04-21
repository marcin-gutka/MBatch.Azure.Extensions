using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;
using Microsoft.Extensions.Logging;

namespace MBatch.Azure.Extensions
{
    public static class CloudPoolExtensions
    {
        public static async Task SetTargetNodesCountAsync(this CloudPool pool, int targetNodeCount, ComputeNodeDeallocationOption computeNodeDeallocationOption, ILogger? logger)
        {
            await pool.RefreshAsync();

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

                await pool.ResizeAsync(targetNodeCount, deallocationOption: computeNodeDeallocationOption);
            }

            // use this instead of ResizeAsync when removing nodes to choose exactly which nodes are removed.
            if (pool.TargetDedicatedComputeNodes > targetNodeCount)
            {
                logger?.LogInformation("New target node count: {TargetNodeCount}, previous: {TargetDedicatedComputeNodes} - removing nodes", targetNodeCount, pool.TargetDedicatedComputeNodes);

                var nodesToDelete = NodesToDelete(pool, targetNodeCount);

                await pool.RemoveFromPoolAsync(nodesToDelete, deallocationOption: computeNodeDeallocationOption);
            }
        }

        private static IEnumerable<string> NodesToDelete(CloudPool pool, int targetNodeCount)
        {
            var nodes = pool.ListComputeNodes().ToList();

            var nodesCount = pool.TargetDedicatedComputeNodes;

            if (nodesCount is null || nodesCount <= targetNodeCount)
                return [];

            var orderedNodes = nodes.OrderBy(x => x.State, new NodeStateComparer());

            return orderedNodes.Take(nodesCount.Value - targetNodeCount).Select(x => x.Id);
        }

        public static async Task RecoverUnhealthyNodesAsync(this CloudPool pool, ILogger? logger)
        {
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
                    case ComputeNodeState.Offline:
                        taskList.Add(node.EnableSchedulingAsync());
                        logger?.LogInformation("Node '{NodeId}' was offline in pool '{PoolId}'. Enabling scheduling was activated.", node.Id, pool.Id);
                        break;
                    case ComputeNodeState.Unusable:
                        taskList.Add(node.RebootAsync(rebootOption: ComputeNodeRebootOption.Requeue));
                        logger?.LogInformation("Node '{NodeId}' was unusable in pool '{PoolId}'. Rebooting was triggered.", node.Id, pool.Id);
                        break;
                    case ComputeNodeState.Unknown:
                        if (pool.AllocationState != AllocationState.Resizing)
                        {
                            taskList.Add(node.RemoveFromPoolAsync(deallocationOption: ComputeNodeDeallocationOption.Requeue));
                            logger?.LogInformation("Node '{NodeId}' had Unkown state in pool '{PoolId}'. Node will be removed.", node.Id, pool.Id);
                        }
                        break;
                }
            }

            if (taskList.Count > 0)
                logger?.LogInformation("Found {TaskListCount} unhealthy nodes in pool '{PoolId}'.", taskList.Count, pool.Id);

            await Task.WhenAll(taskList);
        }

        private static string GetUnhealthyNodesFilter() =>
            "state eq 'offline' or state eq 'unusable' or state eq 'unknown'";

        #region Node State Comparer
        private class NodeStateComparer : IComparer<ComputeNodeState?>
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
                    _ => 20
                };
        }
        #endregion
    }
}
