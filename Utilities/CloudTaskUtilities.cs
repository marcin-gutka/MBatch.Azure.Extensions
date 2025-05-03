using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

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
        /// <param name="isPoolScope">Optional: Sets AutoUserScope. If <see langword="true"/> Pool scope is set, otherwise task scope.</param>
        /// <param name="isAdmin">Optional: Sets Elevation Level. If <see langword="true"/> Elevation level is set to Admin, otherwise NonAdmin.</param>
        /// </summary>
        public static CloudTask CreateTask(string taskId, string commandLine, IList<EnvironmentSetting>? environmentSettings = null, List<string>? dependsOnTaskIds = null, bool? isPoolScope = null, bool? isAdmin = null)
        {
            var task = new CloudTask(taskId, commandLine);

            if (environmentSettings is not null)
                task.EnvironmentSettings = environmentSettings;

            if (dependsOnTaskIds is not null)
                task.DependsOn = TaskDependencies.OnIds(dependsOnTaskIds);

            AutoUserScope autoUserScope = AutoUserScope.Task;

            if (isPoolScope is not null && isPoolScope.Value)
            {
                autoUserScope = AutoUserScope.Pool;
            }

            ElevationLevel autoUserElevationLevel = ElevationLevel.NonAdmin;

            if (isAdmin is not null && isAdmin.Value)
            {
                autoUserElevationLevel = ElevationLevel.Admin;
            }

            task.UserIdentity = new UserIdentity(new AutoUserSpecification(autoUserScope, autoUserElevationLevel));

            return task;
        }
    }
}
