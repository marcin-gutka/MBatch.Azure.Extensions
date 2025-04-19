using Azure.ResourceManager;
using Azure.ResourceManager.Batch;

namespace MBatch.Extensions
{
    public static partial class ArmClientExtensions
    {
        public static BatchAccountResource GetBatchAccountResource(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName, CancellationToken cancellationToken = default)
        {
            var batchResourceId = BatchAccountResource.CreateResourceIdentifier(subscriptionId, resourceGroup, batchAccountName);

            var batchAccountResource = armClient.GetBatchAccountResource(batchResourceId);

            return batchAccountResource.Get(cancellationToken);
        }
    }
}
