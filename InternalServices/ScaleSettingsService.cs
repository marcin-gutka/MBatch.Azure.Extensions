using Azure.ResourceManager.Batch.Models;
using MBatch.Models;

namespace MBatch.InternalServices
{
    internal static class ScaleSettingsService
    {
        internal static BatchAccountPoolScaleSettings CreateScaleSettings(IScaleSettings settings)
        {
            var castedSettings = settings as FixedScaleSettings;

            if (castedSettings is not null)
            {
                return new()
                {
                    FixedScale = new()
                    {
                        TargetDedicatedNodes = castedSettings.TargetDedicatedNodes
                    }
                };
            }

            return new();
        }
    }
}
