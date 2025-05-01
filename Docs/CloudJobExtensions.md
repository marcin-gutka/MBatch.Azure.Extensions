<a name="cloud-job-extensions"></a>
# CloudJobExtensions
* [UpdateAsync](#cloud-job-extensions-update-async)
* [TerminateJobAsync](#cloud-job-extensions-terminate-job-async)
* [IsAnyTaskFailedAsync](#cloud-job-extensions-is-any-task-failed-async)
---

<a name="cloud-job-extensions-update-async"></a>
### `UpdateAsync(CloudJob job, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)`

Updates a `CloudJob` with new values.

#### Parameters:
- **`CloudJob job`**: The `CloudJob` object to update.
- **`string? newJobId`** *(optional)*: New Job ID.
- **`string? newPoolId`** *(optional)*: Pool ID to which the current job is attached.
- **`bool? terminateJobAfterTasksCompleted`** *(optional)*: Set to `true` to terminate the job after completion of all tasks.
- **`bool? useTaskDependencies`** *(optional)*: Set to `true` for task execution ordering.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await job.UpdateAsync(
    newJobId: "NewJob-123",
    newPoolId: "NewPool",
    terminateJobAfterTasksCompleted: true,
    useTaskDependencies: true
);
```
---

<a name="cloud-job-extensions-terminate-job-async"></a>
### `TerminateJobAsync(CloudJob job, CancellationToken cancellationToken = default)`

Terminates a job if all tasks within the job are completed.

#### Parameters:
- **`CloudJob job`**: The `CloudJob` object to check and terminate.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job has been terminated, otherwise: `false`.

#### Example:
```csharp
bool isTerminated = await job.TerminateJobAsync();
```
---

<a name="cloud-job-extensions-is-any-task-failed-async"></a>
### `IsAnyTaskFailedAsync(CloudJob job, CancellationToken cancellationToken = default)`

Checks if any task failed within a job.

#### Parameters:
- **`CloudJob job`**: The `CloudJob` object to evaluate.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if any task within the job failed, otherwise: `false`.

#### Example:
```csharp
bool hasFailedTasks = await job.IsAnyTaskFailedAsync();
```