using Azure.ResourceManager.Batch.Models;

namespace MBatch.Azure.Extensions.Models
{
    public record StartTaskSettings(string CommandLine, bool WaitForSuccess, BatchUserAccountElevationLevel ElevationLevel, BatchAutoUserScope AutoUserScope);
}
