using Microsoft.Azure.Batch;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for <see cref="CloudTask"/> utilities.
    /// </summary>
    public static class CloudTaskUtilities
    {
        /// <summary>
        /// Creates new <see cref="CloudTask"/> object.
        /// <param name="taskId">Task Id.</param>
        /// <param name="commandLine">Task command line to update.</param>
        /// <param name="environmentSettings">Optional: Collection of EnvironmentSetting.</param>
        /// <param name="dependsOnTaskIds">Optional: List of task Ids on which current task is dependent.</param>
        /// </summary>
        public static CloudTask CreateTask(string taskId, string commandLine, IList<EnvironmentSetting>? environmentSettings = null, List<string>? dependsOnTaskIds = null)
        {
            var task = new CloudTask(taskId, commandLine);

            if (environmentSettings is not null)
                task.EnvironmentSettings = environmentSettings;

            if (dependsOnTaskIds is not null)
                task.DependsOn = TaskDependencies.OnIds(dependsOnTaskIds);

            return task;
        }
    }
}
