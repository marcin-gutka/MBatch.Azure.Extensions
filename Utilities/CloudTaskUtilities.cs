using Microsoft.Azure.Batch;

namespace MBatch.Azure.Extensions
{
    public static class CloudTaskUtilities
    {
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
