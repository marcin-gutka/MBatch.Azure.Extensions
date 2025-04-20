using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Batch;
using Azure.ResourceManager.Batch.Models;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for extensions method for ArmClient.
    /// </summary>
    public static partial class ArmClientExtensions
    {
        private const string PACKAGE_FORMAT = "zip";

        /// <summary>
        /// Creates an application package record. Adds new application if it does not exist or new version to exisitng application.
        /// This method calls <see cref="BatchExtensions.GetBatchApplicationPackageResource(ArmClient, ResourceIdentifier)"/> then
        /// it calls <see cref="BatchApplicationPackageResource.UpdateAsync(WaitUntil, BatchApplicationPackageData, CancellationToken)"/>.
        /// </summary>
        /// <param name="armClient">ArmClient to connect to Azure resources.</param>
        /// <param name="subscriptionId">The subscription ID within which the Azure Batch account is located.</param>
        /// <param name="resourceGroup">The resource group name within which the Azure Batch account is located.</param>
        /// <param name="batchAccountName">Batch account name.</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
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

        /// <summary>
        /// Activates created application package.
        /// Prior calling this method the zip package of your application should be uploaded to provided Uri.
        /// Call <see cref="ArmClientExtensions.UpdateBatchApplicationPackage(ArmClient, string, string, string, string, string, bool, CancellationToken)"/> to create application package.
        /// This method calls <see cref="BatchExtensions.GetBatchApplicationPackageResource(ArmClient, ResourceIdentifier)"/> then
        /// it calls <see cref="BatchApplicationPackageResource.ActivateAsync(BatchApplicationPackageActivateContent, CancellationToken)"/>.
        /// </summary>
        /// <param name="armClient">ArmClient to connect to Azure resources.</param>
        /// <param name="subscriptionId">The subscription ID within which the Azure Batch account is located.</param>
        /// <param name="resourceGroup">The resource group name within which the Azure Batch account is located.</param>
        /// <param name="batchAccountName">Batch account name.</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task ActivateBatchApplicationPackage(this ArmClient armClient,
            string subscriptionId, string resourceGroup, string batchAccountName,
            string applicationName, string applicationVersion, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = GetApplicationPackageResource(armClient, subscriptionId, resourceGroup, batchAccountName, applicationName, applicationVersion);

            var content = new BatchApplicationPackageActivateContent(PACKAGE_FORMAT);
            await batchApplicationPackage.ActivateAsync(content, cancellationToken);
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
