using Azure.ResourceManager;
using Azure.ResourceManager.Batch;

namespace MBatch.Azure.Extensions
{
    public static partial class ArmClientExtensions
    {
        /// <summary>
        /// Creates <see cref="BatchAccountResource"/> to interact with Batch Account.
        /// This method calls <see cref="BatchAccountResource.GetAsync(CancellationToken)"/> to fetch Batch Account data.
        /// </summary>
        /// <param name="armClient">ArmClient to connect to Azure resources.</param>
        /// <param name="subscriptionId">The subscription ID within which the Azure Batch account is located.</param>
        /// <param name="resourceGroup">The resource group name within which the Azure Batch account is located.</param>
        /// <param name="batchAccountName">Batch account name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task<BatchAccountResource> GetBatchAccountResourceAsync(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName, CancellationToken cancellationToken = default)
        {
            var batchResourceId = BatchAccountResource.CreateResourceIdentifier(subscriptionId, resourceGroup, batchAccountName);

            var batchAccountResource = armClient.GetBatchAccountResource(batchResourceId);

            return await batchAccountResource.GetAsync(cancellationToken);
        }
    }
}
