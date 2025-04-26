using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Batch;
using Azure.ResourceManager.Batch.Models;
using Azure.ResourceManager.Models;
using MBatch.Azure.Extensions.InternalExtensions;
using MBatch.Azure.Extensions.InternalServices;
using MBatch.Azure.Extensions.Models;
using Microsoft.Azure.Batch;

namespace MBatch.Azure.Extensions
{
    public static partial class BatchAccountResourceExtensions
    {
        #region Create
        /// <summary>
        /// Creates a pool if not exist in Batch Account.
        /// This method calls <see cref="BatchAccountPoolCollection.CreateOrUpdateAsync(WaitUntil, string, BatchAccountPoolData, ETag?, string, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="poolId">Application version.</param>
        /// <param name="vmConfiguration">Chosen Virtual Machine configuration (see <see cref="VMUtilities.MatchVirtualMachineConfiguration(List{ImageInformation}, string?, string?, string?)"/> to choose within Image Informations.
        /// To get Image Informations call <see cref="PoolOperations.ListSupportedImages(DetailLevel, IEnumerable{BatchClientBehavior})"/> using <see cref="BatchClient"/>).</param>
        /// <param name="poolVMSize">Chosen Virtual Machine size (call <see cref="ArmClientExtensions.GetVirtualMachineSize(ArmClient, string, AzureLocation, string, double?, double?, string)"/> to get Virtual Machine size based on chosen SKU or memory/vCPUs).</param>
        /// <param name="applications">Optional: List of applications to be installed in created pool.</param>
        /// <param name="identities">Optional: List of identities to be added for the created pool.</param>
        /// <param name="startTaskModel">Optional: Object representing Start Task for the created pool.</param>
        /// <param name="scaleSettings">Optional: Object representing Scale settings for the created pool. Currently only Fixed Scale is supported (use <see cref="FixedScaleSettings"/> to provide those settings).</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when pool already exists.</exception>
        public static Task CreatePoolAsync(this BatchAccountResource batchAccountResource,
            string poolId,
            VirtualMachineConfiguration vmConfiguration,
            string poolVMSize,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            bool waitUntilCompleted,
            CancellationToken cancellationToken = default)
        {
            var batchDeploymentConfiguration = DeploymentConfigurationService.CreateDeploymentConfiguration(vmConfiguration.ImageReference.Offer, vmConfiguration.ImageReference.Publisher, vmConfiguration.ImageReference.Sku, vmConfiguration.NodeAgentSkuId);

            return batchAccountResource.CreatePoolAsync(poolId,
                batchDeploymentConfiguration, poolVMSize,
                applications, identities, startTaskModel, scaleSettings,
                waitUntilCompleted,
                cancellationToken);
        }

        /// <summary>
        /// Creates a pool if not exist in Batch Account.
        /// This method calls <see cref="BatchAccountPoolCollection.CreateOrUpdateAsync(WaitUntil, string, BatchAccountPoolData, ETag?, string, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="poolId">Application version.</param>
        /// <param name="vmOffer">Virtual Machine offer.</param>
        /// <param name="vmPublisher">Virtual Machine publisher.</param>
        /// <param name="vmSku">Virtual Machine SKU.</param>
        /// <param name="vmNodeAgentSkuId">Virtual Machine node agent SKU Id.</param>
        /// <param name="poolVMSize">Chosen Virtual Machine size (call <see cref="ArmClientExtensions.GetVirtualMachineSize(ArmClient, string, AzureLocation, string, double?, double?, string)"/> to get Virtual Machine size based on chosen SKU or memory/vCPUs).</param>
        /// <param name="applications">Optional: List of applications to be installed in created pool.</param>
        /// <param name="identities">Optional: List of identities to be added for the created pool.</param>
        /// <param name="startTaskModel">Optional: Object representing Start Task for the created pool.</param>
        /// <param name="scaleSettings">Optional: Object representing Scale settings for the created pool. Currently only Fixed Scale is supported (use <see cref="FixedScaleSettings"/> to provide those settings).</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when pool already exists.</exception>
        public static Task CreatePoolAsync(this BatchAccountResource batchAccountResource,
            string poolId,
            string vmOffer, string vmPublisher, string vmSku, string vmNodeAgentSkuId,
            string poolVMSize,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            bool waitUntilCompleted,
            CancellationToken cancellationToken = default)
        {
            var batchDeploymentConfiguration = DeploymentConfigurationService.CreateDeploymentConfiguration(vmOffer, vmPublisher, vmSku, vmNodeAgentSkuId);

            return batchAccountResource.CreatePoolAsync(poolId,
                batchDeploymentConfiguration, poolVMSize,
                applications, identities, startTaskModel, scaleSettings,
                waitUntilCompleted,
                cancellationToken);
        }

        /// <summary>
        /// Creates a pool if not exist in Batch Account.
        /// This method calls <see cref="BatchAccountPoolCollection.CreateOrUpdateAsync(WaitUntil, string, BatchAccountPoolData, ETag?, string, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="poolId">Application version.</param>
        /// <param name="vmConfiguration">Chosen Virtual Machine configuration.</param>
        /// <param name="poolVMSize">Chosen Virtual Machine size (call <see cref="ArmClientExtensions.GetVirtualMachineSize(ArmClient, string, AzureLocation, string, double?, double?, string)"/> to get Virtual Machine size based on chosen SKU or memory/vCPUs).</param>
        /// <param name="applications">Optional: List of applications to be installed in created pool.</param>
        /// <param name="identities">Optional: List of identities to be added for the created pool.</param>
        /// <param name="startTaskModel">Optional: Object representing Start Task for the created pool.</param>
        /// <param name="scaleSettings">Optional: Object representing Scale settings for the created pool. Currently only Fixed Scale is supported (use <see cref="FixedScaleSettings"/> to provide those settings).</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when pool already exists.</exception>
        public static async Task CreatePoolAsync(this BatchAccountResource batchAccountResource,
            string poolId,
            BatchDeploymentConfiguration vmConfiguration,
            string poolVMSize,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            bool waitUntilCompleted,
            CancellationToken cancellationToken = default)
        {
            if (GetPoolAsync(batchAccountResource, poolId, cancellationToken) is not null)
                throw new ArgumentException($"Pool: '{poolId}' already exists.");

            var batchAccountPoolData = new BatchAccountPoolData()
            {
                DisplayName = poolId,
                DeploymentConfiguration = vmConfiguration,
                StartTask = startTaskModel is not null ? StartTaskService.CreateStartTask(startTaskModel.CommandLine, startTaskModel.WaitForSuccess, startTaskModel.ElevationLevel, startTaskModel.AutoUserScope) : null,
                VmSize = poolVMSize,
                ScaleSettings = scaleSettings is not null ? ScaleSettingsService.CreateScaleSettings(scaleSettings) : null,
            };

            if (identities is not null)
                AddIdentity(batchAccountPoolData, identities);

            if (applications is not null)
            {
                foreach (var application in applications)
                    batchAccountPoolData.ApplicationPackages.Add(CreateApplicationReference(application, batchAccountResource));
            }

            var batchAccountPoolCollection = batchAccountResource.GetBatchAccountPools();

            await batchAccountPoolCollection.CreateOrUpdateAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, batchAccountPoolData.DisplayName, batchAccountPoolData, cancellationToken: cancellationToken);
        }

        private static void AddIdentity(BatchAccountPoolData poolData, List<ManagedIdentityInfo> identities)
        {
            poolData.Identity = new(ManagedServiceIdentityType.UserAssigned);

            foreach (var identity in identities)
            {
                var identityResourceId = ResourceIdentifier.Parse(GetManagedIdentityResourceId(identity.IdentityName, identity.SubscriptionId, identity.ResourceGroup));

                poolData.Identity.UserAssignedIdentities.Add(identityResourceId, new());
            }
        }
        private static string GetManagedIdentityResourceId(string managedIdentity, string subscriptionId, string resourceGroup) =>
            $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{managedIdentity}";


        #region for checking if it works
        private static BatchApplicationPackageReference CreateApplicationReference(ApplicationPackageReference application, BatchAccountResource batchAccountResource)
        {
            var resourceId = batchAccountResource.GetBatchAccountApplicationResourceIdentifier(application.ApplicationId);

            return new(resourceId)
            {
                Version = application.Version
            };
        }
        #endregion
        #endregion               

        /// <summary>
        /// Delets existing pool in Batch Account.
        /// This method calls <see cref="BatchAccountResource.GetBatchAccountPoolAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchAccountPoolResource.DeleteAsync(WaitUntil, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="poolId">Application version.</param>
        /// <param name="waitUntilCompleted">If <see langword="true"/> then the method waits for operation to complete, otherwise it returns when operation has started.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when pool is not found.</exception>
        public static async Task DeletePoolAsync(this BatchAccountResource batchAccountResource,
            string poolId, bool waitUntilCompleted, CancellationToken cancellationToken = default)
        {
            var pool = await GetPoolAsync(batchAccountResource, poolId, cancellationToken);

            if (pool is null)
                throw new ArgumentException($"Pool: '{poolId}' has not been found.");

            await pool.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);
        }

        /// <summary>
        /// Updates existing pool in Batch Account.
        /// This method calls <see cref="BatchAccountResource.GetBatchAccountPoolAsync(string, CancellationToken)"/> then
        /// it calls <see cref="BatchAccountPoolResource.UpdateAsync(BatchAccountPoolData, ETag?, CancellationToken)"/>.
        /// </summary>
        /// <param name="batchAccountResource">BatchAccountResource to connect to Batch Account resource.</param>
        /// <param name="poolId">Application version.</param>
        /// <param name="applications">Optional: List of applications to be installed in created pool.</param>
        /// <param name="identities">Optional: List of identities to be added for the created pool.</param>
        /// <param name="startTaskModel">Optional: Object representing Start Task for the created pool.</param>
        /// <param name="scaleSettings">Optional: Object representing Scale settings for the created pool. Currently only Fixed Scale is supported (use <see cref="FixedScaleSettings"/> to provide those settings).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when pool is not found.</exception>
        public static async Task UpdatePoolAsync(this BatchAccountResource batchAccountResource,
            string poolId,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            CancellationToken cancellationToken = default)
        {
            var pool = await GetPoolAsync(batchAccountResource, poolId, cancellationToken);

            if (pool is null)
                throw new ArgumentException($"Pool: '{poolId}' has not been found.");

            var batchAccountPoolData = new BatchAccountPoolData()
            {
                StartTask = startTaskModel is not null ? StartTaskService.CreateStartTask(startTaskModel.CommandLine, startTaskModel.WaitForSuccess, startTaskModel.ElevationLevel, startTaskModel.AutoUserScope) : null,
                ScaleSettings = scaleSettings is not null ? ScaleSettingsService.CreateScaleSettings(scaleSettings) : null,
            };

            if (identities is not null)
                AddIdentity(batchAccountPoolData, identities);

            if (applications is not null)
            {
                batchAccountPoolData.ApplicationPackages.Clear();

                foreach (var application in applications)
                {
                    batchAccountPoolData.ApplicationPackages.Add(CreateApplicationReference(application, batchAccountResource));
                }
            }

            await pool.UpdateAsync(batchAccountPoolData, cancellationToken: cancellationToken);
        }

        private static async Task<BatchAccountPoolResource> GetPoolAsync(BatchAccountResource batchAccountResource, string poolId, CancellationToken cancellationToken = default) =>
            await batchAccountResource.GetBatchAccountPoolAsync(poolId, cancellationToken);
    }
}
