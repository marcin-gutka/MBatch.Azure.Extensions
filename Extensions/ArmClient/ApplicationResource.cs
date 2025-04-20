using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Batch;
using Azure.ResourceManager.Batch.Models;

namespace MBatch.Azure.Extensions
{
    public static partial class ArmClientExtensions
    {
        private const string PACKAGE_FORMAT = "zip";

        /// <summary>
        /// Creates an application package record. Adds new application if it does not exist or new version to exisitng application.
        /// This method calls <see cref="BatchExtensions.GetBatchApplicationPackageResource(ArmClient, ResourceIdentifier)"/> then
        /// it calls <see cref="BatchApplicationPackageResource.UpdateAsync(WaitUntil, BatchApplicationPackageData, CancellationToken)"/>.
        /// </summary>
        /// <returns><see cref="Uri"/> for uploading application package to blob storage configured in Batch Account</returns>
        public static async Task<Uri> UpdateBatchApplicationPackage(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName,
            string applicationName, string applicationVersion,
            bool waitUntilCompleted = false, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = GetApplicationPackageResource(armClient, subscriptionId, resourceGroup, batchAccountName, applicationName, applicationVersion);

            var data = new BatchApplicationPackageData();

            var response = await batchApplicationPackage.UpdateAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, data, cancellationToken);

            return response.Value.Data.StorageUri;
        }        

        public static async Task<bool> ActivateBatchApplicationPackage(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName,
            string applicationName, string applicationVersion, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = GetApplicationPackageResource(armClient, subscriptionId, resourceGroup, batchAccountName, applicationName, applicationVersion);

            var content = new BatchApplicationPackageActivateContent(PACKAGE_FORMAT);
            await batchApplicationPackage.ActivateAsync(content, cancellationToken);

            return true;
        }

        public static async Task<bool> DeleteBatchApplicationPackage(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName,
            string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = GetApplicationPackageResource(armClient, subscriptionId, resourceGroup, batchAccountName, applicationName, applicationVersion);

            await batchApplicationPackage.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);

            return true;
        }

        public static async Task<bool> DeleteBatchApplication(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName,
            string applicationName, bool waitUntilCompleted, CancellationToken cancellationToken = default)
        {
            var batchApplication = GetApplicationResource(armClient, subscriptionId, resourceGroup, batchAccountName, applicationName);

            var packageCollection = batchApplication.GetBatchApplicationPackages();

            var taskList = new List<Task>();

            foreach (var package in packageCollection)
                taskList.Add(DeleteBatchApplicationPackage(armClient, subscriptionId, resourceGroup, batchAccountName, applicationName, package.Data.Name, true, cancellationToken));

            await Task.WhenAll(taskList);

            await batchApplication.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);

            return true;
        }

        private static BatchApplicationResource GetApplicationResource(ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, string applicationName)
        {
            var batchApplicationResourceId = BatchApplicationResource.CreateResourceIdentifier(subscriptionId, resourceGroup,
               batchAccountName, applicationName);

            return armClient.GetBatchApplicationResource(batchApplicationResourceId);
        }

        private static BatchApplicationPackageResource GetApplicationPackageResource(ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, string applicationName, string applicationVersion)
        {
            var batchApplicationPackageResourceId = BatchApplicationPackageResource.CreateResourceIdentifier(subscriptionId, resourceGroup,
                batchAccountName, applicationName, applicationVersion);

            return armClient.GetBatchApplicationPackageResource(batchApplicationPackageResourceId);
        }
    }
}
