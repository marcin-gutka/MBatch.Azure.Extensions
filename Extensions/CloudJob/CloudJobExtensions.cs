using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for extensions methods for <see cref="CloudJob"/>.
    /// </summary>
    public static class CloudJobExtensions
    {
        /// <summary>
        /// Updates CloudJob with new values.
        /// This method calls <see cref="CloudJob.CommitChangesAsync(IEnumerable{BatchClientBehavior}, CancellationToken)"/>.
        /// </summary>
        /// <param name="job">CloudJob object.</param>
        /// <param name="newJobId">new Job Id.</param>
        /// <param name="newPoolId">Pool Id to which current job is attached.</param>
        /// <param name="terminateJobAfterTasksCompleted">Set to <see langword="true"/> to terminate job after completion of all tasks.</param>
        /// <param name="useTaskDependencies">Set to <see langword="true"/> when task execution ordering is required.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task UpdateAsync(this CloudJob job, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)
        {
            var update = false;

            if (!string.IsNullOrEmpty(newJobId))
            {
                if (job.Id != newJobId)
                {
                    job.Id = newJobId;
                    update = true;
                }
            }

            if (!string.IsNullOrEmpty(newPoolId))
            {
                if (job.PoolInformation.PoolId != newPoolId)
                {
                    job.PoolInformation.PoolId = newPoolId;
                    update = true;
                }
            }

            if (terminateJobAfterTasksCompleted != null)
            {
                var onAllTasksComplete = terminateJobAfterTasksCompleted.Value ? OnAllTasksComplete.TerminateJob : OnAllTasksComplete.NoAction;

                if (job.OnAllTasksComplete != onAllTasksComplete)
                {
                    job.OnAllTasksComplete = onAllTasksComplete;
                    update = true;
                }
            }

            if (useTaskDependencies != null)
            {
                if (job.UsesTaskDependencies != useTaskDependencies)
                {
                    job.UsesTaskDependencies = useTaskDependencies;
                    update = true;
                }
            }

            if (update)
                await job.CommitChangesAsync(cancellationToken: cancellationToken);
        }


        /// <summary>
        /// Terminates job if all tasks within this job are completed.
        /// This method calls following methods <see cref="CloudJob.RefreshAsync(DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudJob.ListTasks(DetailLevel, IEnumerable{BatchClientBehavior})"/>,
        /// <see cref="CloudJob.TerminateAsync(string, IEnumerable{BatchClientBehavior}, CancellationToken)"/>
        /// </summary>
        /// <param name="job">CloudJob object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> if job has been terminated, otherwise <see langword="false"/>.</returns>
        public static async Task<bool> TerminateJobAsync(this CloudJob job, CancellationToken cancellationToken = default)
        {
            // check their statuses in Batch service
            await job.RefreshAsync(cancellationToken: cancellationToken);

            var state = job.State;

            if (state == JobState.Completed)
                return true;

            if (state == JobState.Active)
            {
                var taskList = job.ListTasks();

                if (taskList.All(x => x.State == TaskState.Completed))
                {
                    await job.TerminateAsync("All tasks are completed", cancellationToken: cancellationToken);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if any task failed within a job.
        /// This method calls following methods <see cref="CloudJob.RefreshAsync(DetailLevel, IEnumerable{BatchClientBehavior}, CancellationToken)"/>,
        /// <see cref="CloudJob.ListTasks(DetailLevel, IEnumerable{BatchClientBehavior})"/>
        /// </summary>
        /// <param name="job">CloudJob object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see langword="true"/> if any task failed within a job, otherwise <see langword="false"/>.</returns>
        public static async Task<bool> IsAnyTaskFailedAsync(this CloudJob job, CancellationToken cancellationToken = default)
        {
            await job.RefreshAsync(cancellationToken: cancellationToken);

            var tasks = job.ListTasks();

            return tasks.Any(x => x.ExecutionInformation.FailureInformation?.Code is
                TaskFailureInformationCodes.FailureExitCode or
                TaskFailureInformationCodes.TaskEnded);
        }
    }
}
