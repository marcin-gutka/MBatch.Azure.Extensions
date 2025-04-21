using Azure;
using Azure.ResourceManager.Batch;
using Azure.ResourceManager.Batch.Models;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for extensions methods for <see cref="BatchAccountResource"/>.
    /// </summary>
    public static partial class BatchAccountResourceExtensions
    {
        const string PACKAGE_FORMAT = "zip";

        /// <summary>
        /// Creates an application package record. Adds new application if it does not exist or new version to exisitng application.
        /// This method calls <see cref="BatchAccountResource.GetBatchApplicationAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchApplicationResource.GetBatchApplicationPackageAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchApplicationPackageResource.UpdateAsync(WaitUntil, BatchApplicationPackageData, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Uri"/> for uploading application package to blob storage configured in Batch Account</returns>
        public static async Task<Uri> UpdateApplicationPackageAsync(this BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)
        {
            var applicationPackageResource = await GetBatchApplicationPackageResourceAsync(batchAccountResource, applicationName, applicationVersion, cancellationToken);

            var data = new BatchApplicationPackageData();

            var response = await applicationPackageResource.UpdateAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, data, cancellationToken);

            return response.Value.Data.StorageUri;
        }

        /// <summary>
        /// Activates created application package (application version).
        /// Prior calling this method the zip package of your application should be uploaded to provided Uri.
        /// Call <see cref="BatchAccountResourceExtensions.UpdateApplicationPackageAsync(BatchAccountResource, string, string, bool, CancellationToken)"/> to create application package.
        /// This method calls <see cref="BatchAccountResource.GetBatchApplicationAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchApplicationResource.GetBatchApplicationPackageAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchApplicationPackageResource.ActivateAsync(BatchApplicationPackageActivateContent, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task ActivateApplicationPackageAsync(this BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = await GetBatchApplicationPackageResourceAsync(batchAccountResource, applicationName, applicationVersion, cancellationToken);

            var content = new BatchApplicationPackageActivateContent(PACKAGE_FORMAT);
            await batchApplicationPackage.ActivateAsync(content, cancellationToken);
        }

        /// <summary>
        /// Deletes an application package record (application version) and uploaded application package in the storage.
        /// This method calls <see cref="BatchAccountResource.GetBatchApplicationAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchApplicationResource.GetBatchApplicationPackageAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchApplicationPackageResource.DeleteAsync(WaitUntil, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task DeleteApplicationPackageAsync(this BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)
        {
            var batchApplicationPackage = await GetBatchApplicationPackageResourceAsync(batchAccountResource, applicationName, applicationVersion, cancellationToken);

            await batchApplicationPackage.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);
        }

        /// <summary>
        /// Deletes an application, all its application package records and uploaded application packages in the storage.
        /// This method calls <see cref="BatchAccountResource.GetBatchApplicationAsync(string, CancellationToken)"/> then for each application package
        /// it calls <see cref="BatchApplicationPackageResource.DeleteAsync(WaitUntil, CancellationToken)"/>
        /// finally it calls <see cref="BatchApplicationResource.DeleteAsync(WaitUntil, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task DeleteApplicationAsync(this BatchAccountResource batchAccountResource,
            string applicationName, bool waitUntilCompleted, CancellationToken cancellationToken = default)
        {
            var batchApplication = (await batchAccountResource.GetBatchApplicationAsync(applicationName, cancellationToken)).Value;

            ArgumentNullException.ThrowIfNull(batchApplication);

            var packageCollection = batchApplication.GetBatchApplicationPackages();

            var taskList = new List<Task>();

            foreach (var package in packageCollection)
                taskList.Add(batchAccountResource.DeleteApplicationPackageAsync(applicationName, package.Data.Name, true, cancellationToken));

            await Task.WhenAll(taskList);

            await batchApplication.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);
        }

        private static async Task<BatchApplicationPackageResource> GetBatchApplicationPackageResourceAsync(BatchAccountResource batchAccountResource,
            string applicationName, string applicationVersion, CancellationToken cancellationToken = default)
        {
            var applicationResource = await batchAccountResource.GetBatchApplicationAsync(applicationName, cancellationToken);

            ArgumentNullException.ThrowIfNull(applicationResource.Value);

            return await applicationResource.Value.GetBatchApplicationPackageAsync(applicationVersion, cancellationToken);
        }
    }
}
