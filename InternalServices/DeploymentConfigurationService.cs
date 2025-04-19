using Azure.ResourceManager.Batch.Models;

namespace MBatch.InternalServices
{
    public static class DeploymentConfigurationService
    {
        public static BatchDeploymentConfiguration CreateDeploymentConfiguration(string offer, string publisher, string sku, string nodeAgentSkuId) =>
            new()
            {
                VmConfiguration = new BatchVmConfiguration(imageReference: new BatchImageReference()
                {
                    Offer = offer,
                    Publisher = publisher,
                    Sku = sku
                },
                nodeAgentSkuId: nodeAgentSkuId)
            };
    }
}
