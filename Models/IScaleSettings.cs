namespace MBatch.Models
{
    public interface IScaleSettings
    {
    }

    public record FixedScaleSettings(int TargetDedicatedNodes) : IScaleSettings;
}
