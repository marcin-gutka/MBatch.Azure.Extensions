using Azure.ResourceManager.Batch.Models;
using Microsoft.Azure.Batch.Common;

namespace MBatch.Azure.Extensions.Models
{
    /// <summary>
    /// Model to provide pool start task.
    /// </summary>
    /// <param name="CommandLine">Start task command line.</param>
    /// <param name="WaitForSuccess">When set to <see langword="true"/> pool stays in <see cref="ComputeNodeState.WaitingForStartTask"/> until start task is completed after starting.</param>
    /// <param name="ElevationLevel">Set priviliges to run a start task.</param>
    /// <param name="AutoUserScope">Set task execution for new user specified for start task or general autouser account across each node.</param>
    public record StartTaskSettings(string CommandLine, bool WaitForSuccess, BatchUserAccountElevationLevel ElevationLevel, BatchAutoUserScope AutoUserScope);
}
