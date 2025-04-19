using Azure.Core;
using Azure.ResourceManager.Batch;

namespace MBatch.InternalExtensions
{
    internal static class BatchAccountResourceIdentifiersExtenstions
    {
        internal static ResourceIdentifier GetBatchAccountApplicationResourceIdentifier(this BatchAccountResource batchAccountResource,
            string applicationName) =>
            ResourceIdentifier.Parse($"{batchAccountResource.Data.AccountEndpoint}/applications/{applicationName}");
    }
}
