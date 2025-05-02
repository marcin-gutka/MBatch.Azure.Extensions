<a name="batch-client-extensions"></a>
# BatchClientExtensions
* [Job](#batch-client-job)
* [Pool](#batch-client-pool)
* [Task](#batch-client-task)
---

<a name="batch-client-job"></a>
## Job
* [GetJobAsync](#batch-client-job-get-async)
* [CreateJobAsync](#batch-client-job-create-async)
* [DeleteJobIfExistsAsync](#batch-client-job-delete-if-exists-async)
* [UpdateJobAsync](#batch-client-job-update-async)
* [TerminateJobAsync](#batch-client-job-terminate-async)
* [GetRunningJobsAsync](#batch-client-job-get-running-jobs-async)
* [IsAnyTaskFailedAsync](#batch-client-job-is-any-task-failed-async)
* [GetJobsTasksCountsAsync](#batch-client-job-get-jobs-tasks-counts-async)
---

<a name="batch-client-job-get-async"></a>
### `GetJobAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Gets a job from a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`CloudJob`** when a job is found, otherwise **`null`**.

#### Exceptions:
- **`BatchException`**: Passed through, except when the job is not found.

#### Example:
```csharp
CloudJob? job = await batchClient.GetJobAsync(
    jobId: "Job-123",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-create-async"></a>
### `CreateJobAsync(BatchClient batchClient, string poolId, string jobId, bool terminateJobAfterTasksCompleted = false, bool useTaskDependencies = false, CancellationToken cancellationToken = default)`

Creates a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool to which this job is attached.
- **`string jobId`**: Job identifier.
- **`bool terminateJobAfterTasksCompleted`** *(optional)*: Set to `true` to complete the job after all tasks are completed.
- **`bool useTaskDependencies`** *(optional)*: Set to `true` when task execution ordering is required.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job is created, `false` if the job already exists.

#### Exceptions:
- **`BatchException`**: Passed through, except when the job already exists.

#### Remarks:
- `terminateJobAfterTasksCompleted` set to `true` terminates the job immediately if no tasks are attached. It is recommended to set this flag after tasks are added.

#### Example:
```csharp
bool isCreated = await batchClient.CreateJobAsync(
    poolId: "MyPool",
    jobId: "Job-123",
    terminateJobAfterTasksCompleted: true,
    useTaskDependencies: false
);
```
---

<a name="batch-client-job-delete-if-exists-async"></a>
### `DeleteJobIfExistsAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Deletes a job from a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job is deleted, otherwise `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the job is not found.

#### Example:
```csharp
bool isDeleted = await batchClient.DeleteJobIfExistsAsync(
    jobId: "Job-123",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-update-async"></a>
### `UpdateJobAsync(BatchClient batchClient, string jobId, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)`

Updates a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string? newJobId`** *(optional)*: New job identifier.
- **`string? newPoolId`** *(optional)*: New pool to which the job is attached.
- **`bool? terminateJobAfterTasksCompleted`** *(optional)*: Set to `true` to complete the job after all tasks are completed.
- **`bool? useTaskDependencies`** *(optional)*: Set to `true` when task execution ordering is required.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Remarks:
- `terminateJobAfterTasksCompleted` set to `true` terminates the job immediately if no tasks are attached. It is recommended to set this flag after tasks are added.

#### Example:
```csharp
await batchClient.UpdateJobAsync(
    jobId: "Job-123",
    newJobId: "Job-124",
    newPoolId: "MyPool",
    terminateJobAfterTasksCompleted: true,
    useTaskDependencies: false,
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-terminate-async"></a>
### `TerminateJobAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Terminates a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job is terminated, `false` if the job does not exist.

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Example:
```csharp
bool isTerminated = await batchClient.TerminateJobAsync(
    jobId: "Job-123",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-get-running-jobs-async"></a>
### `GetRunningJobsAsync(BatchClient batchClient, string poolId)`

Gets running jobs for a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.

#### Returns:
**`List<string>`**: List of running job IDs.

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Example:
```csharp
List<string> runningJobs = batchClient.GetRunningJobsAsync("MyPool");
```
---

<a name="batch-client-job-is-any-task-failed-async"></a>
### `IsAnyTaskFailedAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Checks if any task failed within a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if any task within a job failed; otherwise, `false`.

#### Exceptions:
- **`BatchException`**: Passed through as-is.
- **`ArgumentException`**: Thrown when the job is not found.

#### Example:
```csharp
bool hasFailedTasks = await batchClient.IsAnyTaskFailedAsync(
    jobId: "Job-123"
);
```
---

<a name="batch-client-job-get-jobs-tasks-counts-async"></a>
### `GetJobsTasksCountsAsync(BatchClient batchClient, IEnumerable<string> jobsIds, CancellationToken cancellationToken = default)`

Gets tasks counts for jobs in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`IEnumerable<string> jobsIds`**: Collection of job IDs.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`IEnumerable<TaskCountsResult>`**: Collection of `TaskCountsResult` for the provided job IDs.

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Example:
```csharp
IEnumerable<TaskCountsResult> taskCounts = await batchClient.GetJobsTasksCountsAsync(
    jobsIds: new List<string> { "Job-123", "Job-124" }
);
```
---

<a name="batch-client-pool"></a>
## Pool
* [DoesPoolExistAsync](#batch-client-pool-does-exist-async)
* [GetPoolJobs](#batch-client-pool-get-pool-jobs)
* [DeletePoolIfExistsAsync](#batch-client-pool-delete-pool-if-exists-async)
* [RebootNodesAsync(BatchClient, string, ComputeNodeRebootOption, CancellationToken)](#batch-client-pool-reboot-nodes-async1)
* [RebootNodesAsync(BatchClient, CloudPool, ComputeNodeRebootOption, CancellationToken)](#batch-client-pool-reboot-nodes-async2)
---

<a name="batch-client-pool-does-exist-async"></a>
### `DoesPoolExistAsync(BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)`

Checks if a pool exists in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the pool exists, otherwise `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Example:
```csharp
bool poolExists = await batchClient.DoesPoolExistAsync(
    poolId: "MyPool",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-pool-get-pool-jobs"></a>
### `GetPoolJobs(BatchClient batchClient, string poolId)`

Gets all jobs for a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.

#### Returns:
**`IEnumerable<CloudJob>`**: Collection of `CloudJob`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Example:
```csharp
IEnumerable<CloudJob> poolJobs = batchClient.GetPoolJobs(poolId: "MyPool");
```
---

<a name="batch-client-pool-delete-pool-if-exists-async"></a>
### `DeletePoolIfExistsAsync(BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)`

Deletes a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the pool is deleted; otherwise, `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Example:
```csharp
bool isDeleted = await batchClient.DeletePoolIfExistsAsync(
    poolId: "MyPool"
);
```
---

<a name="batch-client-pool-reboot-nodes-async1"></a>
### `RebootNodesAsync(BatchClient batchClient, string poolId, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)`

Reboots nodes within a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.
- **`ComputeNodeRebootOption computeNodeRebootOption`**: Action to execute when a node is rebooted.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Remarks:
- Waits up to 5 minutes for the pool to reach a steady state if needed.

#### Example:
```csharp
await batchClient.RebootNodesAsync(
    poolId: "MyPool",
    computeNodeRebootOption: ComputeNodeRebootOption.Requeue
);
```
---

<a name="batch-client-pool-reboot-nodes-async2"></a>
### `RebootNodesAsync(BatchClient batchClient, CloudPool pool, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)`

Reboots nodes within a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`CloudPool pool`**: Pool object.
- **`ComputeNodeRebootOption computeNodeRebootOption`**: Action to execute when a node is rebooted.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Remarks:
- Waits up to 5 minutes for the pool to reach a steady state if needed.

#### Example:
```csharp
await batchClient.RebootNodesAsync(
    pool: myPool,
    computeNodeRebootOption: ComputeNodeRebootOption.Requeue
);
```
---

<a name="batch-client-task"></a>
## Task
* [GetTaskAsync](#batch-client-task-get-async)
* [CommitTaskAsync](#batch-client-task-commit-task-async)
* [CommitTasksAsync](#batch-client-task-commit-tasks-async)
* [DeleteTaskAsync](#batch-client-task-delete-task-async)
* [UpdateTaskAsync](#batch-client-task-update-task-async)
---

<a name="batch-client-task-get-async"></a>
### `GetTaskAsync(BatchClient batchClient, string jobId, string taskId, CancellationToken cancellationToken = default)`

Gets a task from a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string taskId`**: Task identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`CloudTask`**: when task is found, otherwise: `null`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the task is not found.

#### Example:
```csharp
CloudTask? task = await batchClient.GetTaskAsync(
    jobId: "Job-123",
    taskId: "Task-456"
);
```
---

<a name="batch-client-task-commit-task-async"></a>
### `CommitTaskAsync(BatchClient batchClient, string jobId, CloudTask task, bool setTerminateJob, CancellationToken cancellationToken = default)`

Commits a task for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CloudTask task`**: Task object.
- **`bool setTerminateJob`**: Set to `true` to terminate the job after completion of all tasks.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the task is committed, otherwise: `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the task already exists.

#### Example:
```csharp
bool isTaskCommitted = await batchClient.CommitTaskAsync(
    jobId: "Job-123",
    task: myTask,
    setTerminateJob: true
);
```
---

<a name="batch-client-task-commit-tasks-async"></a>
### `CommitTasksAsync(BatchClient batchClient, string jobId, List<CloudTask> tasks, bool setTerminateJob, CancellationToken cancellationToken = default)`

Commits tasks for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`List<CloudTask> tasks`**: List of task objects.
- **`bool setTerminateJob`**: Set to `true` to terminate the job after completion of all tasks.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if tasks are committed, otherwise: `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when a task already exists.

#### Example:
```csharp
bool areTasksCommitted = await batchClient.CommitTasksAsync(
    jobId: "Job-123",
    tasks: new List<CloudTask> { task1, task2 },
    setTerminateJob: true
);
```
---

<a name="batch-client-task-delete-task-async"></a>
### `DeleteTaskAsync(BatchClient batchClient, string jobId, string taskId, CancellationToken cancellationToken = default)`

Deletes a task for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string taskId`**: Task identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the task is deleted, otherwise: `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the task is not found.

#### Example:
```csharp
bool isTaskDeleted = await batchClient.DeleteTaskAsync(
    jobId: "Job-123",
    taskId: "Task-456"
);
```