using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    public static partial class BatchClientExtensions
    {
        /// <summary>
        /// Gets a task from a job in a Batch Account.
        /// This method calls <see cref="JobOperations.GetTaskAsync(string, string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="taskId">Task Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="CloudTask"/> when a task is found, otherwise <see langword="null"/></returns>
        /// <exception cref="BatchException">Passing through except when a task is not found.</exception>
        public static async Task<CloudTask?> GetTaskAsync(this BatchClient batchClient, string jobId, string taskId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await batchClient.JobOperations.GetTaskAsync(jobId, taskId, cancellationToken: cancellationToken);
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.TaskNotFound)
                    return null;

                throw;
            }
        }

        /// <summary>
        /// Commit a task for a job in a Batch Account.
        /// This method calls <see cref="JobOperations.AddTaskAsync(string, CloudTask, System.Collections.Concurrent.ConcurrentDictionary{Type, IFileStagingArtifact}, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// If setTerminateJob is set to <see langword="true"/>, also following methods are called: <see cref="JobOperations.GetJobAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudJob.CommitChangesAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="task">Task object.</param>
        /// <param name="setTerminateJob">Set to <see langword="true"/> to terminate job after completion of all tasks.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when a task is commited, otherwise <see langword="false"/></returns>
        /// <exception cref="BatchException">Passing through except when a task already exists.</exception>
        public static async Task<bool> CommitTaskAsync(this BatchClient batchClient, string jobId, CloudTask task, bool setTerminateJob, CancellationToken cancellationToken = default)
        {
            try
            {
                await batchClient.JobOperations.AddTaskAsync(jobId, task, cancellationToken: cancellationToken);

                if (setTerminateJob)
                    await SetJobTerminationAsync(batchClient, jobId, cancellationToken);

                return true;
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.TaskExists)
                    return false;

                throw;
            }
        }

        /// <summary>
        /// Commit tasks for a job in a Batch Account.
        /// This method calls <see cref="JobOperations.AddTaskAsync(string, IEnumerable{CloudTask}, BatchClientParallelOptions, System.Collections.Concurrent.ConcurrentBag{System.Collections.Concurrent.ConcurrentDictionary{Type, IFileStagingArtifact}}, TimeSpan?, IEnumerable{BatchClientBehavior})"/>.
        /// If setTerminateJob is set to <see langword="true"/>, also following methods are called: <see cref="JobOperations.GetJobAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudJob.CommitChangesAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// If operation fails due to BatchException, the method will try to revert already added tasks calling <see cref="JobOperations.DeleteTaskAsync(string, string, IEnumerable{BatchClientBehavior}, CancellationToken)"/> for each taskId.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="tasks">List of task objects.</param>
        /// <param name="setTerminateJob">Set to <see langword="true"/> to terminate job after completion of all tasks.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when tasks are commited, otherwise <see langword="false"/></returns>
        /// <exception cref="BatchException">Passing through except when a task already exists.</exception>
        public static async Task<bool> CommitTasksAsync(this BatchClient batchClient, string jobId, List<CloudTask> tasks, bool setTerminateJob, CancellationToken cancellationToken = default)
        {
            try
            {
                await batchClient.JobOperations.AddTaskAsync(jobId, tasks);

                if (setTerminateJob)
                    await SetJobTerminationAsync(batchClient, jobId, cancellationToken);

                return true;
            }
            catch (BatchException e)
            {
                await RevertAddedTasksAsync(batchClient, jobId, tasks);

                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.TaskExists)
                    return false;

                throw;
            }
        }

        /// <summary>
        /// Delete a task for a job in a Batch Account.
        /// This method calls <see cref="JobOperations.DeleteTaskAsync(string, string, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="taskId">Task Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when task is deleted, otherwise <see langword="false"/></returns>
        /// <exception cref="BatchException">Passing through except when a task is not found.</exception>
        public static async Task<bool> DeleteTaskAsync(this BatchClient batchClient, string jobId, string taskId, CancellationToken cancellationToken = default)
        {
            try
            {
                await batchClient.JobOperations.DeleteTaskAsync(jobId, taskId, cancellationToken: cancellationToken);
                return true;
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.TaskNotFound)
                    return false;

                throw;
            }
        }

        private static async Task SetJobTerminationAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            await batchClient.UpdateJobAsync(jobId, terminateJobAfterTasksCompleted: true, cancellationToken: cancellationToken);
        }

        private static async Task RevertAddedTasksAsync(BatchClient batchClient, string jobId, List<CloudTask> tasks)
        {
            var taskList = new List<Task>();

            foreach (var task in tasks)
            {
                taskList.Add(DeleteTaskAsync(batchClient, jobId, task.Id));
            }

            await Task.WhenAll(taskList);
        }
    }
}
