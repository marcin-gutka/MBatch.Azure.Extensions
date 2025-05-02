using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for extensions methods for <see cref="BatchClient"/>.
    /// </summary>
    public static partial class BatchClientExtensions
    {
        /// <summary>
        /// Gets a job from a Batch Account.
        /// This method calls <see cref="JobOperations.GetJobAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="CloudJob"/> when a job is found, otherwise <see langword="null"/></returns>
        /// <exception cref="BatchException">Passing through except when a job is not found.</exception>
        public static async Task<CloudJob?> GetJobAsync(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
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

        /// <summary>
        /// Creates a job in a Batch Account.
        /// This method calls <see cref="CloudJob.CommitAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="poolId">Pool to which, this job is attached.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="terminateJobAfterTasksCompleted">Set to <see langword="true"/> for complete job after all tasks are completed.</param>
        /// <param name="useTaskDependencies">Set to <see langword="true"/> when task execution ordering is required.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when a job is created, <see langword="false"/> when a job already existed.</returns>
        /// <exception cref="BatchException">Passing through except when a job already exists.</exception>
        /// <remarks>terminateJobAfterTasksCompleted set to <see langword="true"/> terminates job immediately if there are no task attached. It is recommended to set this flag after tasks are added.</remarks>
        public static async Task<bool> CreateJobAsync(this BatchClient batchClient, string poolId, string jobId, bool terminateJobAfterTasksCompleted = false, bool useTaskDependencies = false, CancellationToken cancellationToken = default)
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

        /// <summary>
        /// Deletes a job from a Batch Account.
        /// This method calls <see cref="JobOperations.DeleteJobAsync(string, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when a job is deleted, otherwise <see langword="false"/>.</returns>
        /// <exception cref="BatchException">Passing through except when a job is not found.</exception>
        public static async Task<bool> DeleteJobIfExistsAsync(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                await batchClient.JobOperations.DeleteJobAsync(jobId, cancellationToken: cancellationToken);
                return true;
            }
            catch (BatchException e)
            {
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobNotFound)
                    return false;

                throw;
            }
        }

        /// <summary>
        /// Updates a job in a Batch Account.
        /// This method calls <see cref="CloudJob.CommitAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="newJobId">new job Id.</param>
        /// <param name="newPoolId">new pool to which job is attached.</param>
        /// <param name="terminateJobAfterTasksCompleted">Set to <see langword="true"/> for complete job after all tasks are completed.</param>
        /// <param name="useTaskDependencies">Set to <see langword="true"/> when task execution ordering is required.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="BatchException">Passing through exception.</exception>
        /// <remarks>terminateJobAfterTasksCompleted set to <see langword="true"/> terminates job immediately if there are no task attached. It is recommended to set this flag after tasks are added.</remarks>
        public static async Task UpdateJobAsync(this BatchClient batchClient, string jobId, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)
        {
            var job = await GetJobAsync(batchClient, jobId, cancellationToken);

            if (job is null)
                return;

            await job.UpdateAsync(newJobId, newPoolId, terminateJobAfterTasksCompleted, useTaskDependencies, cancellationToken);
        }

        /// <summary>
        /// Termintes a job in a Batch Account.
        /// This method calls <see cref="CloudJob.TerminateAsync(string, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when a job is termianted, <see langword="false"/> when a job do not exist.</returns>
        /// <exception cref="BatchException">Passing through exception.</exception>
        public static async Task<bool> TerminateJobAsync(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            var job = await GetJobAsync(batchClient, jobId, cancellationToken);

            if (job is null)
                return false;

            return await job.TerminateJobAsync(cancellationToken);
        }

        /// <summary>
        /// Gets running jobs for a pool in a Batch Account.
        /// This method calls <see cref="JobOperations.ListJobs(DetailLevel, IEnumerable{BatchClientBehavior})"/> then for each job within the pool
        /// it calls <see cref="CloudJob.ListTasks(DetailLevel, IEnumerable{BatchClientBehavior})"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="poolId">Pool Id.</param>
        /// <returns><see langword="List"/> of <see langword="string"/> with running jobs ids.</returns>
        /// <exception cref="BatchException">Passing through exception.</exception>
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

        /// <summary>
        /// Checks if any task failed within a job in a Batch Account.
        /// This method calls <see cref="JobOperations.GetJobAsync(string, DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/> then
        /// it calls <see cref="CloudJob.ListTasks(DetailLevel, IEnumerable{BatchClientBehavior})"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobId">Job Id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> when any task within a job failed, otherwise <see langword="false"/>.</returns>
        /// <exception cref="BatchException">Passing through exception.</exception>
        /// <exception cref="ArgumentException">Thrown when Job is not found</exception>
        public static async Task<bool> IsAnyTaskFailedAsync(this BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)
        {
            var job = await GetJobAsync(batchClient, jobId, cancellationToken) ??
                    throw new ArgumentException("Job: '{JobId}' not found", jobId);

            return await job.IsAnyTaskFailedAsync(cancellationToken);
        }

        /// <summary>
        /// Gets tasks counts for jobs in a Batch Account.
        /// This method calls for each job <see cref="JobOperations.GetJobTaskCountsAsync(string, IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchClient">BatchClient to connect to Batch Account.</param>
        /// <param name="jobsIds">Collection of jobs ids.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of <see cref="TaskCountsResult"/> for provided jobs ids.</returns>
        /// <exception cref="BatchException">Passing through exception.</exception>
        public static async Task<IEnumerable<TaskCountsResult>> GetJobsTasksCountsAsync(this BatchClient batchClient, IEnumerable<string> jobsIds, CancellationToken cancellationToken = default)
        {
            var taskList = new List<Task<TaskCountsResult>>();

            foreach (var jobId in jobsIds)
                taskList.Add(batchClient.JobOperations.GetJobTaskCountsAsync(jobId, cancellationToken: cancellationToken));

            return await Task.WhenAll(taskList);
        }
    }
}
