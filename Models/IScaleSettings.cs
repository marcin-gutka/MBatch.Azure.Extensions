namespace MBatch.Azure.Extensions.Models
{
    public interface IScaleSettings
    {
    }

    public record FixedScaleSettings(int TargetDedicatedNodes) : IScaleSettings;
}
