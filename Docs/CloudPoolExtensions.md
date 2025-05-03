<a name="cloud-pool-extensions"></a>
# CloudPoolExtensions
* [SetTargetNodesCountAsync](#cloud-pool-extensions-set-target-nodes-count-async)
* [RecoverUnhealthyNodesAsync](#cloud-pool-extensions-recover-unhealthy-nodes-async)
---

<a name="cloud-pool-extensions-set-target-nodes-count-async"></a>
### `SetTargetNodesCountAsync(CloudPool pool, int targetNodeCount, ComputeNodeDeallocationOption computeNodeDeallocationOption, ILogger? logger, CancellationToken cancellationToken = default)`

Sets the number of nodes for a pool.

#### Parameters:
- **`CloudPool pool`**: The `CloudPool` object to modify.
- **`int targetNodeCount`**: Number of nodes to set.
- **`ComputeNodeDeallocationOption computeNodeDeallocationOption`**: Action to perform on tasks when a node is removed.
- **`ILogger? logger`** *(optional)*: Logger for logging operations.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Remarks:
- When removing nodes, the custom comparer is used to determine which nodes are most efficient to remove.
- AutoScale must be disabled.
- Low-priority nodes are not currently supported by this extension.

#### Example:
```csharp
await pool.SetTargetNodesCountAsync(
    targetNodeCount: 2,
    ComputeNodeDeallocationOption.Requeue,
    logger: myLogger
);
```
---

<a name="cloud-pool-extensions-recover-unhealthy-nodes-async"></a>
### `RecoverUnhealthyNodesAsync(CloudPool pool, ILogger? logger, CancellationToken cancellationToken = default)`

Recovers unhealthy nodes in a pool.

#### Parameters:
- **`CloudPool pool`**: The `CloudPool` object containing the nodes.
- **`ILogger? logger`** *(optional)*: Logger for logging operations.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`int`**: Number of unhealthy nodes.

#### Remarks:
- By providing 
- For `ComputeNodeState.Offline` -> enabling scheduling.
- For `ComputeNodeState.Deallocated` -> enabling scheduling.
- For `ComputeNodeState.Unusable` -> reboot.
- For `ComputeNodeState.Unknown` -> node removal if pool is not in `AllocationState.Resizing` state.

#### Example:
```csharp
await pool.RecoverUnhealthyNodesAsync(logger: myLogger);
```