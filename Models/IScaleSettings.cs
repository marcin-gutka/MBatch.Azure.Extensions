namespace MBatch.Azure.Extensions.Models
{
    /// <summary>
    /// Interface to setting pool scale settings.
    /// </summary>
    /// <remarks>Currently only fixed scale is supported.</remarks>
    public interface IScaleSettings
    {
    }

    /// <summary>
    /// Model to provide pool fixed scale settings.
    /// </summary>
    /// <param name="TargetDedicatedNodes">Sets target dedicated nodes</param>
    /// <remarks>Low priority nodes are not currently supported by this extension.</remarks>
    public record FixedScaleSettings(int TargetDedicatedNodes) : IScaleSettings;
}
