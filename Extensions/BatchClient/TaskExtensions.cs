using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    public static partial class BatchClientExtensions
    {
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

        public static async Task UpdateTaskAsync(this BatchClient batchClient, string jobId, string taskId, string? commandLine, IList<EnvironmentSetting>? environmentSettings = null, List<string>? dependsOnTaskIds = null, CancellationToken cancellationToken = default)
        {
            var task = await GetTaskAsync(batchClient, jobId, taskId, cancellationToken);

            if (task is null)
                return;

            var update = false;

            if (!string.IsNullOrWhiteSpace(commandLine))
            {
                if (task.CommandLine != commandLine)
                {
                    task.CommandLine = commandLine;
                    update = true;
                }
            }

            if (environmentSettings is not null)
            {
                task.EnvironmentSettings = environmentSettings;
                update = true;
            }

            if (dependsOnTaskIds is not null)
            {
                task.DependsOn = TaskDependencies.OnIds(dependsOnTaskIds);
                update = true;
            }

            if (update)
            {
                await task.CommitAsync(cancellationToken: cancellationToken);

                // check in test if it works

                /*var job = await GetJob(batchClient, jobId, cancellationToken);

                if (job is null)
                    return;

                await DeleteTask(batchClient, jobId, taskId, cancellationToken);

                job.AddTask(task);
                await job.CommitChangesAsync(cancellationToken: cancellationToken);*/
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
