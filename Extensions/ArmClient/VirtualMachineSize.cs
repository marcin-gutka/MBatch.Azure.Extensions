using Azure.ResourceManager;
using Azure.ResourceManager.Batch;
using Azure.ResourceManager.Batch.Models;
using Azure.ResourceManager.Resources;

namespace MBatch.Azure.Extensions
{
    public static partial class ArmClientExtensions
    {
        private const string MEMORY_CAPABILITY_NAME = "MemoryGB";
        private const string VCPUS_CAPABILITY_NAME = "vCPUs";

        // TODO: Provide proper method description
        public static string GetVirtualMachineSize(this ArmClient armClient,
            string subscriptionId, string location,
            string? skuName = null, string? familyName = null,
            double? minMemory = null, double? minvCPUs = null)
        {
            var supportedSkus = GetSupportedSkus(armClient, subscriptionId, location, familyName);

            var chosenSku = supportedSkus.FirstOrDefault(x => x.Name == skuName);

            chosenSku ??= ChooseSkuBasedOnHardware(supportedSkus, minMemory, minvCPUs);

            if (chosenSku is null)
                throw new ArgumentException($"Chosen VM size is not available in subscription '{subscriptionId}' or within location '{location}'." +
                    $" Please provide MinMemory and/or MinvCPUs");

            return chosenSku.Name;
        }

        private static List<BatchSupportedSku> GetSupportedSkus(ArmClient armClient, string subscriptionId, string location, string? familyName)
        {
            var subscriptionResourceId = SubscriptionResource.CreateResourceIdentifier(subscriptionId);

            var subscriptionResource = armClient.GetSubscriptionResource(subscriptionResourceId);

            var availableSkus = subscriptionResource.GetBatchSupportedVirtualMachineSkus(location, filter: familyName is not null ? $"familyName eq '{familyName}'" : null).ToList();

            return availableSkus;
        }

        private static BatchSupportedSku? ChooseSkuBasedOnHardware(List<BatchSupportedSku> supportedSkus, double? minMemory, double? minvCPUs)
        {
            if (minMemory is null && minvCPUs is null)
                return null;

            if (minMemory is not null && minvCPUs is not null)
                return supportedSkus.Where(x => GetCapability(x, MEMORY_CAPABILITY_NAME) >= minMemory.Value && GetCapability(x, VCPUS_CAPABILITY_NAME) >= minvCPUs.Value)
                    .OrderBy(x => GetCapability(x, MEMORY_CAPABILITY_NAME)).ThenBy(x => GetCapability(x, VCPUS_CAPABILITY_NAME))
                    .First();

            if (minMemory is not null)
                return supportedSkus.Where(x => GetCapability(x, MEMORY_CAPABILITY_NAME) >= minMemory.Value)
                    .OrderBy(x => GetCapability(x, MEMORY_CAPABILITY_NAME)).ThenBy(x => GetCapability(x, VCPUS_CAPABILITY_NAME))
                    .First();

            if (minvCPUs is not null)
                return supportedSkus.Where(x => GetCapability(x, VCPUS_CAPABILITY_NAME) >= minvCPUs.Value)
                        .OrderBy(x => GetCapability(x, VCPUS_CAPABILITY_NAME)).ThenBy(x => GetCapability(x, MEMORY_CAPABILITY_NAME))
                        .First();

            return null;
        }

        private readonly static Func<BatchSupportedSku, string, double> GetCapability = (sku, capabilityName) =>
            double.Parse(sku.Capabilities.First(x => x.Name == capabilityName).Value);
    }
}
