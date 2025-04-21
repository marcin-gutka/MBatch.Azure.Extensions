using Azure;
using Azure.Core;
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
        public static Task CreatePool(this BatchAccountResource batchAccountResource,
            VirtualMachineConfiguration vmConfiguration,
            string poolId, string poolVMSize,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            bool waitUntilCompleted,
            CancellationToken cancellationToken = default)
        {
            var batchDeploymentConfiguration = DeploymentConfigurationService.CreateDeploymentConfiguration(vmConfiguration.ImageReference.Offer, vmConfiguration.ImageReference.Publisher, vmConfiguration.ImageReference.Sku, vmConfiguration.NodeAgentSkuId);

            return batchAccountResource.CreatePool(batchDeploymentConfiguration,
                poolId, poolVMSize, applications, identities, startTaskModel, scaleSettings, waitUntilCompleted, cancellationToken);
        }

        public static Task CreatePool(this BatchAccountResource batchAccountResource,
            string vmOffer, string vmPublisher, string vmSku, string vmNodeAgentSkuId,
            string poolId, string poolVMSize,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            bool waitUntilCompleted,
            CancellationToken cancellationToken = default)
        {
            var batchDeploymentConfiguration = DeploymentConfigurationService.CreateDeploymentConfiguration(vmOffer, vmPublisher, vmSku, vmNodeAgentSkuId);

            return batchAccountResource.CreatePool(batchDeploymentConfiguration,
               poolId, poolVMSize, applications, identities, startTaskModel, scaleSettings, waitUntilCompleted, cancellationToken);
        }

        public static async Task CreatePool(this BatchAccountResource batchAccountResource,
            BatchDeploymentConfiguration vmConfiguration,
            string poolId, string poolVMSize,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            bool waitUntilCompleted,
            CancellationToken cancellationToken = default)
        {
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

        public static async Task DeletePool(this BatchAccountResource batchAccountResource,
            string poolId, bool waitUntilCompleted, CancellationToken cancellationToken = default)
        {
            var pool = await GetPool(batchAccountResource, poolId, cancellationToken);

            await pool.DeleteAsync(waitUntilCompleted ? WaitUntil.Completed : WaitUntil.Started, cancellationToken);
        }

        public static async Task UpdatePool(this BatchAccountResource batchAccountResource,
            string poolId,
            List<ApplicationPackageReference>? applications,
            List<ManagedIdentityInfo>? identities,
            StartTaskSettings? startTaskModel,
            IScaleSettings? scaleSettings,
            CancellationToken cancellationToken = default)
        {
            var pool = await GetPool(batchAccountResource, poolId, cancellationToken);

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

        private static async Task<BatchAccountPoolResource> GetPool(BatchAccountResource batchAccountResource, string poolId, CancellationToken cancellationToken = default) =>
            await batchAccountResource.GetBatchAccountPoolAsync(poolId, cancellationToken) ??
                throw new ArgumentException($"Pool: '{poolId}' has not been found.");
    }
}
