using Azure.Core;
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

        /// <summary>
        /// Gets Virtual Machine Size from available SKUs for subscription and location optionally filtered by family name. If provided skuName matches any supported sku then it is chosen, if not then sku based on hardware requirements is matched if provided.
        /// First minumum required memory and minimum required vCPUs is matched, then second try is minumum required memory and the lowest possible vCPU and finally minimum required vCPU and the lowest possible memory.
        /// Any of those 3 optional parameters needs to be provided in order to select any SKU.
        /// This method calls <see cref="BatchExtensions.GetBatchSupportedVirtualMachineSkus(SubscriptionResource, AzureLocation, int?, string, CancellationToken)"/>.
        /// </summary>
        /// <param name="armClient">ArmClient to connect to Azure resources.</param>
        /// <param name="subscriptionId">The subscription ID within which the Azure Batch account is located.</param>
        /// <param name="location">The Azure location.</param>
        /// <param name="skuName">Optional: Desired sku to be chosen.</param>
        /// <param name="minMemory">Optional: to choose sku within avaialable skus based on minimum memory requirement.</param>
        /// <param name="minvCPUs">Optional: to choose sku within avaialable skus based on minimum vCPUs requirement.</param>
        /// <param name="familyName">Optional: to filter available skus within subscription and location.</param>
        /// <returns><see cref="string"/> of selected SKU name</returns>
        /// <exception cref="ArgumentException">Thrown when none of skuName, minMemory, minvCPUs allows SKU selection.</exception>
        public static string GetVirtualMachineSize(this ArmClient armClient,
            string subscriptionId, AzureLocation location,
            string? skuName = null,
            double? minMemory = null, double? minvCPUs = null,
            string? familyName = null)
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
