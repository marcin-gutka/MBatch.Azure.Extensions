using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions
{
    public static class CloudJobExtensions
    {
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

        public static async Task<bool> TerminateJobAsync(this CloudJob job, CancellationToken cancellationToken = default)
        {
            // check their statuses in Batch service
            var state = job.State;

            if (state == JobState.Completed)
                return true;

            if (state == JobState.Active)
            {
                // if finished, mark job as completed

                var taskList = job.ListTasks().ToList();

                if (taskList.All(x => x.State == TaskState.Completed))
                {
                    await job.TerminateAsync("All tasks are completed", cancellationToken: cancellationToken);
                    return true;
                }
            }

            return false;
        }

        public static bool IsAnyTaskFailed(this CloudJob job)
        {
            var tasks = job.ListTasks();

            return tasks.Any(x => x.ExecutionInformation.FailureInformation?.Code is
                TaskFailureInformationCodes.FailureExitCode or
                TaskFailureInformationCodes.TaskEnded);
        }
    }
}
