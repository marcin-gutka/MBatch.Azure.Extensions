using Azure.ResourceManager.Batch.Models;

namespace MBatch.InternalServices
{
    internal static class StartTaskService
    {
        internal static BatchAccountPoolStartTask CreateStartTask(string commandLine, bool waitForSuccess, BatchUserAccountElevationLevel elevationLevel = BatchUserAccountElevationLevel.NonAdmin, BatchAutoUserScope autoUserScope = BatchAutoUserScope.Pool) =>
            new()
            {
                CommandLine = commandLine,
                WaitForSuccess = waitForSuccess,
                UserIdentity = new BatchUserIdentity()
                {
                    AutoUser = new BatchAutoUserSpecification()
                    {
                        ElevationLevel = elevationLevel,
                        Scope = autoUserScope
                    }
                }
            };
    }
}
