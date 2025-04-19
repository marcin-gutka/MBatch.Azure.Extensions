using MBatch.Extensions;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Extensions
{
    public static partial class BatchClientExtensions
    {
        public static async Task<CloudJob?> GetJob(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await batchClient.JobOperations.GetJobAsync(jobId, cancellationToken: cancellationToken);
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobNotFound)
                    return null;

                throw;
            }
        }

        public static async Task<bool> CreateJob(this BatchClient batchClient, string poolId, string jobId, bool terminateJobAfterTasksCompleted = false, bool useTaskDependencies = false, CancellationToken cancellationToken = default)
        {
            try
            {
                CloudJob job = batchClient.JobOperations.CreateJob();
                job.Id = jobId;
                job.PoolInformation = new PoolInformation { PoolId = poolId };
                job.OnAllTasksComplete = terminateJobAfterTasksCompleted ? OnAllTasksComplete.TerminateJob : OnAllTasksComplete.NoAction;
                job.UsesTaskDependencies = useTaskDependencies;
                await job.CommitAsync(cancellationToken: cancellationToken);

                return true;
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobExists)
                    return false;

                throw;
            }
        }

        public static async Task DeleteJob(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                await batchClient.JobOperations.DeleteJobAsync(jobId, cancellationToken: cancellationToken);
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobNotFound)
                    return;

                throw;
            }
        }

        public static async Task UpdateJob(this BatchClient batchClient, string jobId, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)
        {
            var job = await GetJob(batchClient, jobId, cancellationToken);

            if (job is null)
                return;

            await job.Update(newJobId, newPoolId, terminateJobAfterTasksCompleted, useTaskDependencies, cancellationToken);
        }        

        public static async Task<bool> TerminateJob(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            var job = await GetJob(batchClient, jobId, cancellationToken);

            if (job is null)
                return false;

            return await job.TerminateJob(cancellationToken);
        }

        public static List<string> GetRunningJobs(this BatchClient batchClient, string poolId)
        {
            var jobs = batchClient.JobOperations.ListJobs();

            var jobsIds = new List<string>();

            foreach (var job in jobs)
            {
                if (job.PoolInformation.PoolId == poolId)
                {
                    if (job.State == JobState.Active)
                    {
                        var tasks = job.ListTasks();

                        if (tasks.Any(x => x.State is TaskState.Running or TaskState.Completed or TaskState.Preparing))
                            jobsIds.Add(job.Id);
                    }
                }
            }

            return jobsIds;
        }

        public static async Task<bool> IsAnyTaskFailed(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                var job = await GetJob(batchClient, jobId, cancellationToken) ??
                    throw new ArgumentException("Job: '{JobId}' not found", jobId);

                return job.IsAnyTaskFailed();
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobNotFound)
                    return false;

                throw;
            }
        }

        public static async Task<TaskCountsResult[]> GetJobsTasksCount(this BatchClient batchClient, IEnumerable<string> jobsIds, CancellationToken cancellationToken = default)
        {
            var taskList = new List<Task<TaskCountsResult>>();

            foreach (var jobId in jobsIds)
                taskList.Add(batchClient.JobOperations.GetJobTaskCountsAsync(jobId, cancellationToken: cancellationToken));

            return await Task.WhenAll(taskList);
        }
    }
}
