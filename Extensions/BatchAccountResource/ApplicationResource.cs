using Azure;
using Azure.ResourceManager.Batch;
using Azure.ResourceManager.Batch.Models;

namespace MBatch.Extensions
{
    public static partial class BatchAccountResourceExtensions
    {
        const string PACKAGE_FORMAT = "zip";

        public static async Task<Uri> UpdateApplicationPackage(this BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)
        {
           var applicationPackageResource = await GetBatchApplicationPackageResource(batchAccountResource, applicationName, applicationVersion, cancellationToken);

            var data = new BatchApplicationPackageData();

            var response = await applicationPackageResource.UpdateAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, data, cancellationToken);

            return response.Value.Data.StorageUri;
        }

        public static async Task<bool> ActivateApplicationPackage(this BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = await GetBatchApplicationPackageResource(batchAccountResource, applicationName, applicationVersion, cancellationToken);

            var content = new BatchApplicationPackageActivateContent(PACKAGE_FORMAT);
            await batchApplicationPackage.ActivateAsync(content, cancellationToken);

            return true;
        }

        public static async Task<bool> DeleteApplicationPackage(this BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = await GetBatchApplicationPackageResource(batchAccountResource, applicationName, applicationVersion, cancellationToken);

            await batchApplicationPackage.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);

            return true;
        }

        public static async Task<bool> DeleteApplication(this BatchAccountResource batchAccountResource,
            string applicationName, bool waitUntilCompleted, CancellationToken cancellationToken = default)
        {
            var batchApplication = (await batchAccountResource.GetBatchApplicationAsync(applicationName, cancellationToken)).Value;

            ArgumentNullException.ThrowIfNull(batchApplication);

            var packageCollection = batchApplication.GetBatchApplicationPackages();

            var taskList = new List<Task>();

            foreach (var package in packageCollection)
                taskList.Add(batchAccountResource.DeleteApplicationPackage(applicationName, package.Data.Name, true, cancellationToken));

            await Task.WhenAll(taskList);

            await batchApplication.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);

            return true;
        }

        private static async Task<BatchApplicationPackageResource> GetBatchApplicationPackageResource(BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, CancellationToken cancellationToken = default)
        {
            var applicationResource = await batchAccountResource.GetBatchApplicationAsync(applicationName, cancellationToken);

            ArgumentNullException.ThrowIfNull(applicationResource.Value);

            return await applicationResource.Value.GetBatchApplicationPackageAsync(applicationVersion, cancellationToken);
        }
    }
}
