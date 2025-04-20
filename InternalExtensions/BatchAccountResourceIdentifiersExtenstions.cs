using Azure.Core;
using Azure.ResourceManager.Batch;

namespace MBatch.Extensions.InternalExtensions
{
    internal static class BatchAccountResourceIdentifiersExtenstions
    {
        internal static ResourceIdentifier GetBatchAccountApplicationResourceIdentifier(this BatchAccountResource batchAccountResource,
            string applicationName) =>
            ResourceIdentifier.Parse($"{batchAccountResource.Data.AccountEndpoint}/applications/{applicationName}");
    }
}
