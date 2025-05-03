
<a name="batch-account-resource-extensions"></a>
# BatchAccountResourceExtensions
* [Pool](#batch-acount-resource-pool)
---

<a name="batch-acount-resource-pool"></a>
## Pool
* [CreatePoolAsync(BatchAccountResource, string, VirtualMachineConfiguration, string, List<ApplicationPackageReference>?, List<ManagedIdentityInfo>?, StartTaskSettings?, IScaleSettings?, bool, CancellationToken)](#batch-acount-resource-pool-create-async1)
* [CreatePoolAsync(BatchAccountResource, string, string, string, string, string, string, List<ApplicationPackageReference>?, List<ManagedIdentityInfo>?, StartTaskSettings?, IScaleSettings?, bool, CancellationToken)](#batch-acount-resource-pool-create-async2)
* [CreatePoolAsync(BatchAccountResource, string, BatchDeploymentConfiguration, string, List<ApplicationPackageReference>?, List<ManagedIdentityInfo>?, StartTaskSettings?, IScaleSettings?, bool, CancellationToken)](#batch-acount-resource-pool-create-async3)
* [DeletePoolAsync](#batch-acount-resource-pool-delete-async)
* [UpdatePoolAsync](#batch-acount-resource-pool-update-async)
---

<a name="batch-acount-resource-pool-create-async1"></a>
### `CreatePoolAsync(BatchAccountResource batchAccountResource, string poolId, VirtualMachineConfiguration vmConfiguration, string poolVMSize, List<ApplicationPackageReference>? applications, List<ManagedIdentityInfo>? identities, StartTaskSettings? startTaskModel, IScaleSettings? scaleSettings, bool waitUntilCompleted, CancellationToken cancellationToken = default)`

Creates a pool in the Batch Account if it does not already exist.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string poolId`**: Pool identifier.
- **`VirtualMachineConfiguration vmConfiguration`**: Chosen Virtual Machine configuration. Use [`VMUtilities.MatchVirtualMachineConfiguration(List<ImageInformation>, string?, string?, string?)`](#) to choose within Image Informations. To get Image Informations, call `PoolOperations.ListSupportedImages(DetailLevel, IEnumerable<BatchClientBehavior>)`using `BatchClient`.
- **`string poolVMSize`**: Chosen Virtual Machine size. Call [`ArmClientExtensions.GetVirtualMachineSize(ArmClient, string, AzureLocation, string, double?, double?, string)`](#get-virtual-machine-size) to get Virtual Machine size based on chosen SKU or memory/vCPUs.
- **`List<ApplicationPackageReference>? applications`** *(optional)*: List of applications to be installed in the created pool.
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the created pool. See [`ManagedIdentityInfo`](#models-managed-identity-info).
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the created pool. See [`StartTaskSettings`](#models-start-task-settings).
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the created pool. Currently, only Fixed Scale is supported (use [`FixedScaleSettings`](#models-iscale-settings) to provide these settings).
- **`bool waitUntilCompleted`**: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`ArgumentException`**: Thrown if the pool already exists.

#### Example:
```csharp
await batchAccountResource.CreatePoolAsync(
    poolId: "MyPool",
    vmConfiguration: myVmConfiguration,
    poolVMSize: "Standard_D2_v3",
    applications: new List<ApplicationPackageReference> { appPackage },
    identities: new List<ManagedIdentityInfo> { identity },
    startTaskModel: myStartTaskModel,
    scaleSettings: myScaleSettings,
    waitUntilCompleted: true
);
```
---

<a name="batch-acount-resource-pool-create-async2"></a>
### `CreatePoolAsync(BatchAccountResource batchAccountResource, string poolId, string vmOffer, string vmPublisher, string vmSku, string vmNodeAgentSkuId, string poolVMSize, List<ApplicationPackageReference>? applications, List<ManagedIdentityInfo>? identities, StartTaskSettings? startTaskModel, IScaleSettings? scaleSettings, bool waitUntilCompleted, CancellationToken cancellationToken = default)`

Creates a pool in the Batch Account if it does not already exist.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string poolId`**: Pool identifier.
- **`string vmOffer`**: Virtual Machine offer.
- **`string vmPublisher`**: Virtual Machine publisher.
- **`string vmSku`**: Virtual Machine SKU.
- **`string vmNodeAgentSkuId`**: Virtual Machine node agent SKU Id.
- **`string poolVMSize`**: Chosen Virtual Machine size. Use [`ArmClientExtensions.GetVirtualMachineSize(ArmClient, string, AzureLocation, string, double?, double?, string)`](#get-virtual-machine-size) to get Virtual Machine size based on chosen SKU or memory/vCPUs.
- **`List<ApplicationPackageReference>? applications`** *(optional)*: List of applications to be installed in the created pool.
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the created pool. See [`ManagedIdentityInfo`](#models-managed-identity-info).
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the created pool. See [`StartTaskSettings`](#models-start-task-settings).
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the created pool. Currently, only Fixed Scale is supported (use [`FixedScaleSettings`](#models-iscale-settings) to provide these settings).
- **`bool waitUntilCompleted`**: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`ArgumentException`**: Thrown if the pool already exists.

#### Example:
```csharp
await batchAccountResource.CreatePoolAsync(
    poolId: "MyPool",
    vmOffer: "windowsserver",
    vmPublisher: "microsoftwindowsserver",
    vmSku: "2016-datacenter-smalldisk",
    vmNodeAgentSkuId: "batch.node.windows amd64",
    poolVMSize: "Standard_A1_v2",
    applications: new List<ApplicationPackageReference> { appPackage },
    identities: new List<ManagedIdentityInfo> { identity },
    startTaskModel: myStartTaskModel,
    scaleSettings: myScaleSettings,
    waitUntilCompleted: true
);
```
---

<a name="batch-acount-resource-pool-create-async3"></a>
### `CreatePoolAsync(BatchAccountResource batchAccountResource, string poolId, BatchDeploymentConfiguration vmConfiguration, string poolVMSize, List<ApplicationPackageReference>? applications, List<ManagedIdentityInfo>? identities, StartTaskSettings? startTaskModel, IScaleSettings? scaleSettings, bool waitUntilCompleted, CancellationToken cancellationToken = default)`

Creates a pool in the Batch Account if it does not already exist.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string poolId`**: Pool identifier.
- **`BatchDeploymentConfiguration vmConfiguration`**: Chosen Virtual Machine configuration.
- **`string poolVMSize`**: Chosen Virtual Machine size. Use [`ArmClientExtensions.GetVirtualMachineSize(ArmClient, string, AzureLocation, string, double?, double?, string)`](#get-virtual-machine-size) to get Virtual Machine size based on chosen SKU or memory/vCPUs.
- **`List<ApplicationPackageReference>? applications`** *(optional)*: List of applications to be installed in the created pool.
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the created pool. See [`ManagedIdentityInfo`](#models-managed-identity-info).
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the created pool. See [`StartTaskSettings`](#models-start-task-settings).
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the created pool. Currently, only Fixed Scale is supported (use [`FixedScaleSettings`](#models-iscale-settings) to provide these settings).
- **`bool waitUntilCompleted`**: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`ArgumentException`**: Thrown if the pool already exists.

#### Example:
```csharp
await batchAccountResource.CreatePoolAsync(
    poolId: "MyPool",
    vmConfiguration: myVmConfiguration,
    poolVMSize: "Standard_D2_v3",
    applications: new List<ApplicationPackageReference> { appPackage },
    identities: new List<ManagedIdentityInfo> { identity },
    startTaskModel: myStartTaskModel,
    scaleSettings: myScaleSettings,
    waitUntilCompleted: true
);
```
---

<a name="batch-acount-resource-pool-delete-async"></a>
### `DeletePoolAsync(BatchAccountResource batchAccountResource, string poolId, bool waitUntilCompleted, CancellationToken cancellationToken = default)`

Deletes an existing pool in the Batch Account.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string poolId`**: Pool identifier.
- **`bool waitUntilCompleted`**: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`ArgumentException`**: Thrown if the pool is not found.

#### Example:
```csharp
await batchAccountResource.DeletePoolAsync(
    poolId: "MyPool",
    waitUntilCompleted: true
);
```
---

<a name="batch-acount-resource-pool-update-async"></a>
### `UpdatePoolAsync(BatchAccountResource batchAccountResource, string poolId, List<ApplicationPackageReference>? applications, List<ManagedIdentityInfo>? identities, StartTaskSettings? startTaskModel, IScaleSettings? scaleSettings, CancellationToken cancellationToken = default)`

Updates an existing pool in the Batch Account.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string poolId`**: Pool identifier.
- **`List<ApplicationPackageReference>? applications`** *(optional)*: List of applications to be installed in the pool.
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the pool.
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the pool.
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the pool. Currently, only Fixed Scale is supported (use `FixedScaleSettings` to provide these settings).
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`ArgumentException`**: Thrown if the pool is not found.

#### Example:
```csharp
await batchAccountResource.UpdatePool(
    poolId: "MyPool",
    applications: new List<ApplicationPackageReference> { appPackage },
    identities: new List<ManagedIdentityInfo> { identity },
    startTaskModel: myStartTaskModel,
    scaleSettings: myScaleSettings
);
```