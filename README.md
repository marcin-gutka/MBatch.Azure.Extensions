# MBatch.Azure.Extensions

This is a nuget package for extending ArmClient and BatchClient objects by extensions methods to interact with Azure Batch Account. It also includes few static utilities methods.

* [ArmClientExtensions](#arm-client-extensions)
* [BatchAccountResourceExtensions](#batch-account-resource-extensions)
* [BatchClientExtensions](#batch-client-extensions)
* [CloudJobExtensions](#cloud-job-extensions)
* [CloudJobExtensions](#cloud-pool-extensions)
* [Utilities](#utilities)

<a name="arm-client-extensions"></a>
# ArmClientExtensions
* [Application Resource](#arm-application-resource)
* [Batch Account Resource](#arm-batch-account-resource)
* [Virtual Machine Size](#arm-virtual-machine-size)
---

<a name="arm-application-resource"></a>
## Application Resource
* [UpdateBatchApplicationPackageAsync](#update-batch-application-package-async)
* [ActivateBatchApplicationPackageAsync](#activate-batch-application-package-async)
* [DeleteBatchApplicationPackageAsync](#delete-batch-application-package-async)
* [DeleteBatchApplicationAsync](#delete-batch-application-async)
---
<a name="update-batch-application-package-async"></a>
### `UpdateBatchApplicationPackageAsync(this ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)`

Creates an application package record. Adds a new application if it does not exist, or a new version to an existing application.

#### Parameters:
- **`ArmClient armClient`**: The `ArmClient` to connect to Azure resources.
- **`string subscriptionId`**: The subscription ID within which the Azure Batch account is located.
- **`string resourceGroup`**: The resource group name within which the Azure Batch account is located.
- **`string batchAccountName`**: Batch account name.
- **`string applicationName`**: Application name.
- **`string applicationVersion`**: Application version.
- **`bool waitUntilCompleted`** *(optional)*: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
- **`Uri`**: A URI for uploading the application package to the blob storage configured in the Batch Account.

#### Example:
```csharp
Uri uploadUri = await armClient.UpdateBatchApplicationPackageAsync(
    subscriptionId: "12345-abcde-67890",
    resourceGroup: "MyResourceGroup",
    batchAccountName: "MyBatchAccount",
    applicationName: "MyApp",
    applicationVersion: "1.0",
    waitUntilCompleted: true
);
```
---
<a name="activate-batch-application-package-async"></a>
### `ActivateBatchApplicationPackageAsync(this ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, string applicationName, string applicationVersion, CancellationToken cancellationToken = default)`

Activates a created application package (application version).  

Before calling this method, the zip package of your application should be uploaded to the provided URI.  
Use [`ArmClientExtensions.UpdateBatchApplicationPackageAsync`](#update-batch-application-package-async) to create the application package.

#### Parameters:
- **`ArmClient armClient`**: The `ArmClient` to connect to Azure resources.
- **`string subscriptionId`**: The subscription ID within which the Azure Batch account is located.
- **`string resourceGroup`**: The resource group name within which the Azure Batch account is located.
- **`string batchAccountName`**: Batch account name.
- **`string applicationName`**: Application name.
- **`string applicationVersion`**: Application version.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await armClient.ActivateBatchApplicationPackageAsync(
    subscriptionId: "12345-abcde-67890",
    resourceGroup: "MyResourceGroup",
    batchAccountName: "MyBatchAccount",
    applicationName: "MyApp",
    applicationVersion: "1.0"
);
```
---
<a name="delete-batch-application-package-async"></a>
### `DeleteBatchApplicationPackageAsync(this ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default`

Deletes an application package record (application version) and the uploaded application package in storage.

#### Parameters:
- **`ArmClient armClient`**: The `ArmClient` to connect to Azure resources.
- **`string subscriptionId`**: The subscription ID within which the Azure Batch account is located.
- **`string resourceGroup`**: The resource group name within which the Azure Batch account is located.
- **`string batchAccountName`**: Batch account name.
- **`string applicationName`**: Application name.
- **`string applicationVersion`**: Application version.
- **`bool waitUntilCompleted`** *(optional)*: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await armClient.DeleteBatchApplicationPackageAsync(
    subscriptionId: "12345-abcde-67890",
    resourceGroup: "MyResourceGroup",
    batchAccountName: "MyBatchAccount",
    applicationName: "MyApp",
    applicationVersion: "1.0",
    waitUntilCompleted: true
);
```
---
<a name="delete-batch-application-async"></a>
### `DeleteBatchApplicationAsync(this ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, string applicationName, bool waitUntilCompleted, CancellationToken cancellationToken = default)`

Deletes an application, all its application package records, and the uploaded application packages in storage.

#### Parameters:
- **`ArmClient armClient`**: The `ArmClient` to connect to Azure resources.
- **`string subscriptionId`**: The subscription ID within which the Azure Batch account is located.
- **`string resourceGroup`**: The resource group name within which the Azure Batch account is located.
- **`string batchAccountName`**: Batch account name.
- **`string applicationName`**: Application name.
- **`bool waitUntilCompleted`**: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await armClient.DeleteBatchApplicationAsync(
    subscriptionId: "12345-abcde-67890",
    resourceGroup: "MyResourceGroup",
    batchAccountName: "MyBatchAccount",
    applicationName: "MyApp",
    waitUntilCompleted: true
);
```
---
<a name="arm-batch-account-resource"></a>
## Batch Account Resource

* [GetBatchAccountResourceAsync](#get-batch-account-resource-async)
---
<a name="get-batch-account-resource-async"></a>
### `GetBatchAccountResourceAsync(this ArmClient armClient, string subscriptionId, string resourceGroup, string batchAccountName, CancellationToken cancellationToken = default)`

Creates a `BatchAccountResource` to interact with the Batch Account.

#### Parameters:
- **`ArmClient armClient`**: The `ArmClient` to connect to Azure resources.
- **`string subscriptionId`**: The subscription ID within which the Azure Batch account is located.
- **`string resourceGroup`**: The resource group name within which the Azure Batch account is located.
- **`string batchAccountName`**: Batch account name.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task<BatchAccountResource>`**

#### Example:
```csharp
BatchAccountResource batchAccount = await armClient.GetBatchAccountResourceAsync(
    subscriptionId: "12345-abcde-67890",
    resourceGroup: "MyResourceGroup",
    batchAccountName: "MyBatchAccount"
);
```
---
<a name="arm-virtual-machine-size"></a>
## Virtual Machine Size

* [GetVirtualMachineSize](#get-virtual-machine-size)
---
<a name="get-virtual-machine-size"></a>

### `GetVirtualMachineSize(this ArmClient armClient, string subscriptionId, AzureLocation location, string? skuName = null, double? minMemory = null, double? minvCPUs = null, string? familyName = null)`

Gets the Virtual Machine Size from available SKUs for a subscription and location, optionally filtered by family name.  
If the provided `skuName` matches any supported SKU, it is chosen. Otherwise, a SKU is selected based on hardware requirements if provided.

**Matching Logic:**
1. First, the method matches minimum required memory and minimum required vCPUs.
2. If no match, it tries minimum required memory and the lowest possible vCPU.
3. Lastly, it tries minimum required vCPU and the lowest possible memory.

**Note:** At least one of `skuName`, `minMemory`, or `minvCPUs` must be provided to select a SKU.

#### Parameters:
- **`ArmClient armClient`**: The `ArmClient` to connect to Azure resources.
- **`string subscriptionId`**: The subscription ID within which the Azure Batch account is located.
- **`AzureLocation location`**: The Azure location.
- **`string? skuName`** *(optional)*: Desired SKU to be chosen.
- **`double? minMemory`** *(optional)*: To filter SKUs based on minimum memory requirements.
- **`double? minvCPUs`** *(optional)*: To filter SKUs based on minimum vCPU requirements.
- **`string? familyName`** *(optional)*: To filter available SKUs within the subscription and location.

#### Returns:
- **`string`**: The name of the selected SKU.

#### Exceptions:
- **`ArgumentException`**: Thrown when none of `skuName`, `minMemory`, or `minvCPUs` allows SKU selection.

#### Example:
```csharp
string sku = armClient.GetVirtualMachineSize(
    subscriptionId: "12345-abcde-67890",
    location: AzureLocation.WestEurope,
    skuName: "Standard_D2_v3",
    minMemory: 4.0,
    minvCPUs: 2.0
);
```
---

<a name="batch-account-resource-extensions"></a>
# BatchAccountResourceExtensions
* [Application Resource](#batch-acount-resource-application-resource)
* [Pool](#batch-acount-resource-pool)
---

<a name="batch-acount-resource-application-resource"></a>
## Application Resource
* [UpdateApplicationPackageAsync](#update-application-package-async)
* [ActivateApplicationPackageAsync](#activate-application-package-async)
* [DeleteApplicationPackageAsync](#delete-application-package-async)
* [DeleteApplicationAsync](#delete-application-async)
---

<a name="update-application-package-async"></a>
### `UpdateApplicationPackageAsync(BatchAccountResource batchAccountResource, string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)`

Creates an application package record. Adds a new application if it does not exist, or a new version to an existing application.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string applicationName`**: Application name.
- **`string applicationVersion`**: Application version.
- **`bool waitUntilCompleted`** *(optional)*: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
- **`Uri`**: A URI for uploading the application package to the blob storage configured in the Batch Account.

#### Example:
```csharp
Uri uploadUri = await batchAccountResource.UpdateApplicationPackageAsync(
    applicationName: "MyApp",
    applicationVersion: "1.0",
    waitUntilCompleted: true
);
```
---

<a name="activate-application-package-async"></a>
### `ActivateApplicationPackageAsync(BatchAccountResource batchAccountResource, string applicationName, string applicationVersion, CancellationToken cancellationToken = default)`

Activates a created application package (application version).  

Before calling this method, the zip package of your application should be uploaded to the provided URI.  
Use [`BatchAccountResourceExtensions.UpdateApplicationPackageAsync`](#update-application-package-async) to create the application package.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string applicationName`**: Application name.
- **`string applicationVersion`**: Application version.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await batchAccountResource.ActivateApplicationPackageAsync(
    applicationName: "MyApp",
    applicationVersion: "1.0"
);
```
---

<a name="delete-application-package-async"></a>
### `DeleteApplicationPackageAsync(BatchAccountResource batchAccountResource, string applicationName, string applicationVersion, bool waitUntilCompleted = false, CancellationToken cancellationToken = default)`

Deletes an application package record (application version) and the uploaded application package in storage.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string applicationName`**: Application name.
- **`string applicationVersion`**: Application version.
- **`bool waitUntilCompleted`** *(optional)*: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await batchAccountResource.DeleteApplicationPackageAsync(
    applicationName: "MyApp",
    applicationVersion: "1.0",
    waitUntilCompleted: true
);
```
---

<a name="delete-application-async"></a>
### `DeleteApplicationAsync(BatchAccountResource batchAccountResource, string applicationName, bool waitUntilCompleted, CancellationToken cancellationToken = default)`

Deletes an application, all its application package records, and the uploaded application packages in storage.

#### Parameters:
- **`BatchAccountResource batchAccountResource`**: The `BatchAccountResource` to connect to the Batch Account resource.
- **`string applicationName`**: Application name.
- **`bool waitUntilCompleted`**: If `true`, the method waits for the operation to complete; otherwise, it returns when the operation has started.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await batchAccountResource.DeleteApplicationAsync(
    applicationName: "MyApp",
    waitUntilCompleted: true
);
```
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
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the created pool.
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the created pool.
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the created pool. Currently, only Fixed Scale is supported (use `FixedScaleSettings` to provide these settings).
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
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the created pool.
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the created pool.
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the created pool. Currently, only Fixed Scale is supported (use `FixedScaleSettings` to provide these settings).
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
    vmOffer: "UbuntuServer",
    vmPublisher: "Canonical",
    vmSku: "18.04-LTS",
    vmNodeAgentSkuId: "batch.node.ubuntu 18.04",
    poolVMSize: "Standard_D2_v3",
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
- **`List<ManagedIdentityInfo>? identities`** *(optional)*: List of identities to be added for the created pool.
- **`StartTaskSettings? startTaskModel`** *(optional)*: Object representing Start Task for the created pool.
- **`IScaleSettings? scaleSettings`** *(optional)*: Object representing Scale settings for the created pool. Currently, only Fixed Scale is supported (use `FixedScaleSettings` to provide these settings).
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
---

<a name="batch-client-extensions"></a>
# BatchClientExtensions
* [Job](#batch-client-job)
* [Pool](#batch-client-pool)
* [Task](#batch-client-task)
---

<a name="batch-client-job"></a>
## Job
* [GetJobAsync](#batch-client-job-get-async)
* [CreateJobAsync](#batch-client-job-create-async)
* [DeleteJobIfExistsAsync](#batch-client-job-delete-if-exists-async)
* [UpdateJobAsync](#batch-client-job-update-async)
* [TerminateJobAsync](#batch-client-job-terminate-async)
* [GetRunningJobsAsync](#batch-client-job-get-running-jobs-async)
* [IsAnyTaskFailedAsync](#batch-client-job-is-any-task-failed-async)
* [GetJobsTasksCountsAsync](#batch-client-job-get-jobs-tasks-counts-async)
---

<a name="batch-client-job-get-async"></a>
### `GetJobAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Gets a job from a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`CloudJob`** when a job is found, otherwise **`null`**.

#### Exceptions:
- **`BatchException`**: Passed through, except when the job is not found.

#### Example:
```csharp
CloudJob? job = await batchClient.GetJobAsync(
    jobId: "Job-123",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-create-async"></a>
### `CreateJobAsync(BatchClient batchClient, string poolId, string jobId, bool terminateJobAfterTasksCompleted = false, bool useTaskDependencies = false, CancellationToken cancellationToken = default)`

Creates a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool to which this job is attached.
- **`string jobId`**: Job identifier.
- **`bool terminateJobAfterTasksCompleted`** *(optional)*: Set to `true` to complete the job after all tasks are completed.
- **`bool useTaskDependencies`** *(optional)*: Set to `true` when task execution ordering is required.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job is created, `false` if the job already exists.

#### Exceptions:
- **`BatchException`**: Passed through, except when the job already exists.

#### Remarks:
- `terminateJobAfterTasksCompleted` set to `true` terminates the job immediately if no tasks are attached. It is recommended to set this flag after tasks are added.

#### Example:
```csharp
bool isCreated = await batchClient.CreateJobAsync(
    poolId: "MyPool",
    jobId: "Job-123",
    terminateJobAfterTasksCompleted: true,
    useTaskDependencies: false
);
```
---

<a name="batch-client-job-delete-if-exists-async"></a>
### `DeleteJobIfExistsAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Deletes a job from a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job is deleted, otherwise `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the job is not found.

#### Example:
```csharp
bool isDeleted = await batchClient.DeleteJobIfExistsAsync(
    jobId: "Job-123",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-update-async"></a>
### `UpdateJobAsync(BatchClient batchClient, string jobId, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)`

Updates a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string? newJobId`** *(optional)*: New job identifier.
- **`string? newPoolId`** *(optional)*: New pool to which the job is attached.
- **`bool? terminateJobAfterTasksCompleted`** *(optional)*: Set to `true` to complete the job after all tasks are completed.
- **`bool? useTaskDependencies`** *(optional)*: Set to `true` when task execution ordering is required.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Remarks:
- `terminateJobAfterTasksCompleted` set to `true` terminates the job immediately if no tasks are attached. It is recommended to set this flag after tasks are added.

#### Example:
```csharp
await batchClient.UpdateJobAsync(
    jobId: "Job-123",
    newJobId: "Job-124",
    newPoolId: "MyPool",
    terminateJobAfterTasksCompleted: true,
    useTaskDependencies: false,
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-terminate-async"></a>
### `TerminateJobAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Terminates a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job is terminated, `false` if the job does not exist.

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Example:
```csharp
bool isTerminated = await batchClient.TerminateJobAsync(
    jobId: "Job-123",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-job-get-running-jobs-async"></a>
### `GetRunningJobsAsync(BatchClient batchClient, string poolId)`

Gets running jobs for a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.

#### Returns:
**`List<string>`**: List of running job IDs.

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Example:
```csharp
List<string> runningJobs = batchClient.GetRunningJobsAsync("MyPool");
```
---

<a name="batch-client-job-is-any-task-failed-async"></a>
### `IsAnyTaskFailedAsync(BatchClient batchClient, string jobId, CancellationToken cancellationToken = default)`

Checks if any task failed within a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if any task within a job failed; otherwise, `false`.

#### Exceptions:
- **`BatchException`**: Passed through as-is.
- **`ArgumentException`**: Thrown when the job is not found.

#### Example:
```csharp
bool hasFailedTasks = await batchClient.IsAnyTaskFailedAsync(
    jobId: "Job-123"
);
```
---

<a name="batch-client-job-get-jobs-tasks-counts-async"></a>
### `GetJobsTasksCountsAsync(BatchClient batchClient, IEnumerable<string> jobsIds, CancellationToken cancellationToken = default)`

Gets tasks counts for jobs in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`IEnumerable<string> jobsIds`**: Collection of job IDs.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`IEnumerable<TaskCountsResult>`**: Collection of `TaskCountsResult` for the provided job IDs.

#### Exceptions:
- **`BatchException`**: Passed through as-is.

#### Example:
```csharp
IEnumerable<TaskCountsResult> taskCounts = await batchClient.GetJobsTasksCountsAsync(
    jobsIds: new List<string> { "Job-123", "Job-124" }
);
```
---

<a name="batch-client-pool"></a>
## Pool
* [DoesPoolExistAsync](#batch-client-pool-does-exist-async)
* [GetPoolJobs](#batch-client-pool-get-pool-jobs)
* [DeletePoolIfExistsAsync](#batch-client-pool-delete-pool-if-exists-async)
* [RebootNodesAsync(BatchClient, string, ComputeNodeRebootOption, CancellationToken)](#batch-client-pool-reboot-nodes-async1)
* [RebootNodesAsync(BatchClient, CloudPool, ComputeNodeRebootOption, CancellationToken)](#batch-client-pool-reboot-nodes-async2)
---

<a name="batch-client-pool-does-exist-async"></a>
### `DoesPoolExistAsync(BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)`

Checks if a pool exists in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the pool exists, otherwise `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Example:
```csharp
bool poolExists = await batchClient.DoesPoolExistAsync(
    poolId: "MyPool",
    cancellationToken: CancellationToken.None
);
```
---

<a name="batch-client-pool-get-pool-jobs"></a>
### `GetPoolJobs(BatchClient batchClient, string poolId)`

Gets all jobs for a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.

#### Returns:
**`IEnumerable<CloudJob>`**: Collection of `CloudJob`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Example:
```csharp
IEnumerable<CloudJob> poolJobs = batchClient.GetPoolJobs(poolId: "MyPool");
```
---

<a name="batch-client-pool-delete-pool-if-exists-async"></a>
### `DeletePoolIfExistsAsync(BatchClient batchClient, string poolId, CancellationToken cancellationToken = default)`

Deletes a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the pool is deleted; otherwise, `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Example:
```csharp
bool isDeleted = await batchClient.DeletePoolIfExistsAsync(
    poolId: "MyPool"
);
```
---

<a name="batch-client-pool-reboot-nodes-async1"></a>
### `RebootNodesAsync(BatchClient batchClient, string poolId, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)`

Reboots nodes within a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string poolId`**: Pool identifier.
- **`ComputeNodeRebootOption computeNodeRebootOption`**: Action to execute when a node is rebooted.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Remarks:
- Waits up to 5 minutes for the pool to reach a steady state if needed.

#### Example:
```csharp
await batchClient.RebootNodesAsync(
    poolId: "MyPool",
    computeNodeRebootOption: ComputeNodeRebootOption.Requeue
);
```
---

<a name="batch-client-pool-reboot-nodes-async2"></a>
### `RebootNodesAsync(BatchClient batchClient, CloudPool pool, ComputeNodeRebootOption computeNodeRebootOption, CancellationToken cancellationToken = default)`

Reboots nodes within a pool in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`CloudPool pool`**: Pool object.
- **`ComputeNodeRebootOption computeNodeRebootOption`**: Action to execute when a node is rebooted.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through, except when the pool is not found.

#### Remarks:
- Waits up to 5 minutes for the pool to reach a steady state if needed.

#### Example:
```csharp
await batchClient.RebootNodesAsync(
    pool: myPool,
    computeNodeRebootOption: ComputeNodeRebootOption.Requeue
);
```
---

<a name="batch-client-task"></a>
## Task
* [GetTaskAsync](#batch-client-task-get-async)
* [CommitTaskAsync](#batch-client-task-commit-task-async)
* [CommitTasksAsync](#batch-client-task-commit-tasks-async)
* [DeleteTaskAsync](#batch-client-task-delete-task-async)
* [UpdateTaskAsync](#batch-client-task-update-task-async)
---

<a name="batch-client-task-get-async"></a>
### `GetTaskAsync(BatchClient batchClient, string jobId, string taskId, CancellationToken cancellationToken = default)`

Gets a task from a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string taskId`**: Task identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`CloudTask`**: when task is found, otherwise: `null`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the task is not found.

#### Example:
```csharp
CloudTask? task = await batchClient.GetTaskAsync(
    jobId: "Job-123",
    taskId: "Task-456"
);
```
---

<a name="batch-client-task-commit-task-async"></a>
### `CommitTaskAsync(BatchClient batchClient, string jobId, CloudTask task, bool setTerminateJob, CancellationToken cancellationToken = default)`

Commits a task for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`CloudTask task`**: Task object.
- **`bool setTerminateJob`**: Set to `true` to terminate the job after completion of all tasks.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the task is committed, otherwise: `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the task already exists.

#### Example:
```csharp
bool isTaskCommitted = await batchClient.CommitTaskAsync(
    jobId: "Job-123",
    task: myTask,
    setTerminateJob: true
);
```
---

<a name="batch-client-task-commit-tasks-async"></a>
### `CommitTasksAsync(BatchClient batchClient, string jobId, List<CloudTask> tasks, bool setTerminateJob, CancellationToken cancellationToken = default)`

Commits tasks for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`List<CloudTask> tasks`**: List of task objects.
- **`bool setTerminateJob`**: Set to `true` to terminate the job after completion of all tasks.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if tasks are committed, otherwise: `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when a task already exists.

#### Example:
```csharp
bool areTasksCommitted = await batchClient.CommitTasksAsync(
    jobId: "Job-123",
    tasks: new List<CloudTask> { task1, task2 },
    setTerminateJob: true
);
```
---

<a name="batch-client-task-delete-task-async"></a>
### `DeleteTaskAsync(BatchClient batchClient, string jobId, string taskId, CancellationToken cancellationToken = default)`

Deletes a task for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string taskId`**: Task identifier.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the task is deleted, otherwise: `false`.

#### Exceptions:
- **`BatchException`**: Passed through, except when the task is not found.

#### Example:
```csharp
bool isTaskDeleted = await batchClient.DeleteTaskAsync(
    jobId: "Job-123",
    taskId: "Task-456"
);
```
---

<a name="batch-client-task-update-task-async"></a>
### `UpdateTaskAsync(BatchClient batchClient, string jobId, string taskId, string? commandLine = null, IList<EnvironmentSetting>? environmentSettings = null, List<string>? dependsOnTaskIds = null, CancellationToken cancellationToken = default)`

Updates a task for a job in a Batch Account.

#### Parameters:
- **`BatchClient batchClient`**: The `BatchClient` to connect to the Batch Account.
- **`string jobId`**: Job identifier.
- **`string taskId`**: Task identifier.
- **`string? commandLine`** *(optional)*: Task command line to update.
- **`IList<EnvironmentSetting>? environmentSettings`** *(optional)*: Collection of `EnvironmentSetting` objects to update.
- **`List<string>? dependsOnTaskIds`** *(optional)*: List of task IDs that the current task depends on.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Exceptions:
- **`BatchException`**: Passed through, except when the task is not found.

#### Example:
```csharp
await batchClient.UpdateTaskAsync(
    jobId: "Job-123",
    taskId: "Task-456",
    commandLine: "echo Hello World",
    environmentSettings: new List<EnvironmentSetting> { new EnvironmentSetting("VAR_NAME", "VAR_VALUE") },
    dependsOnTaskIds: new List<string> { "Task-1", "Task-2" }
);
```
---

<a name="cloud-job-extensions"></a>
# CloudJobExtensions
* [UpdateAsync](#cloud-job-extensions-update-async)
* [TerminateJobAsync](#cloud-job-extensions-terminate-job-async)
* [IsAnyTaskFailedAsync](#cloud-job-extensions-is-any-task-failed-async)
---

<a name="cloud-job-extensions-update-async"></a>
### `UpdateAsync(CloudJob job, string? newJobId = null, string? newPoolId = null, bool? terminateJobAfterTasksCompleted = null, bool? useTaskDependencies = null, CancellationToken cancellationToken = default)`

Updates a `CloudJob` with new values.

#### Parameters:
- **`CloudJob job`**: The `CloudJob` object to update.
- **`string? newJobId`** *(optional)*: New Job ID.
- **`string? newPoolId`** *(optional)*: Pool ID to which the current job is attached.
- **`bool? terminateJobAfterTasksCompleted`** *(optional)*: Set to `true` to terminate the job after completion of all tasks.
- **`bool? useTaskDependencies`** *(optional)*: Set to `true` for task execution ordering.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Example:
```csharp
await job.UpdateAsync(
    newJobId: "NewJob-123",
    newPoolId: "NewPool",
    terminateJobAfterTasksCompleted: true,
    useTaskDependencies: true
);
```
---

<a name="cloud-job-extensions-terminate-job-async"></a>
### `TerminateJobAsync(CloudJob job, CancellationToken cancellationToken = default)`

Terminates a job if all tasks within the job are completed.

#### Parameters:
- **`CloudJob job`**: The `CloudJob` object to check and terminate.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if the job has been terminated, otherwise: `false`.

#### Example:
```csharp
bool isTerminated = await job.TerminateJobAsync();
```
---

<a name="cloud-job-extensions-is-any-task-failed-async"></a>
### `IsAnyTaskFailedAsync(CloudJob job, CancellationToken cancellationToken = default)`

Checks if any task failed within a job.

#### Parameters:
- **`CloudJob job`**: The `CloudJob` object to evaluate.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`bool`**: `true` if any task within the job failed, otherwise: `false`.

#### Example:
```csharp
bool hasFailedTasks = await job.IsAnyTaskFailedAsync();
```
---

<a name="cloud-pool-extensions"></a>
# CloudPoolExtensions
* [SetTargetNodesCountAsync](#cloud-pool-extensions-set-target-nodes-count-async)
* [RecoverUnhealthyNodesAsync](#cloud-pool-extensions-recover-unhealthy-nodes-async)
---

<a name="cloud-pool-extensions-set-target-nodes-count-async"></a>
### `SetTargetNodesCountAsync(CloudPool pool, int targetNodeCount, ComputeNodeDeallocationOption computeNodeDeallocationOption, ILogger? logger, CancellationToken cancellationToken = default)`

Sets the number of nodes for a pool.

#### Parameters:
- **`CloudPool pool`**: The `CloudPool` object to modify.
- **`int targetNodeCount`**: Number of nodes to set.
- **`ComputeNodeDeallocationOption computeNodeDeallocationOption`**: Action to perform on tasks when a node is removed.
- **`ILogger? logger`** *(optional)*: Logger for logging operations.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Remarks:
- When removing nodes, the custom comparer is used to determine which nodes are most efficient to remove.
- AutoScale must be disabled.
- Low-priority nodes are not currently supported by this extension.

#### Example:
```csharp
await pool.SetTargetNodesCountAsync(
    targetNodeCount: 2,
    ComputeNodeDeallocationOption.Requeue,
    logger: myLogger
);
```
---

<a name="cloud-pool-extensions-recover-unhealthy-nodes-async"></a>
### `RecoverUnhealthyNodesAsync(CloudPool pool, ILogger? logger, CancellationToken cancellationToken = default)`

Recovers unhealthy nodes in a pool.

#### Parameters:
- **`CloudPool pool`**: The `CloudPool` object containing the nodes.
- **`ILogger? logger`** *(optional)*: Logger for logging operations.
- **`CancellationToken cancellationToken`** *(optional)*: A token to cancel the operation.

#### Returns:
**`Task`**

#### Remarks:
- By providing 
- For `ComputeNodeState.Offline` -> enabling scheduling.
- For `ComputeNodeState.Unusable` -> reboot.
- For `ComputeNodeState.Unknown` -> node removal if pool is not in `AllocationState.Resizing` state.

#### Example:
```csharp
await pool.RecoverUnhealthyNodesAsync(
    logger: myLogger
);
```
---

<a name="utilities"></a>
# Utilities
* [Cloud Task](#utilities-cloud-task)
* [CommandLine](#utilities-command-line)
* [VirtualMachine](#utilities-virtual-machine)
---

<a name="utilities-cloud-task"></a>
# Cloud Task Utilities
* [CreateTask](#utilities-cloud-task-create-task)
---

<a name="utilities-cloud-task-create-task"></a>
### `CreateTask(string taskId, string commandLine, IList<EnvironmentSetting>? environmentSettings = null, List<string>? dependsOnTaskIds = null)`

Creates a new `CloudTask` object.

#### Parameters:
- **`string taskId`**: Task identifier.
- **`string commandLine`**: Command line to execute the task.
- **`IList<EnvironmentSetting>? environmentSettings`** *(optional)*: Collection of `EnvironmentSetting` objects.
- **`List<string>? dependsOnTaskIds`** *(optional)*: List of task IDs that the current task depends on.

#### Returns:
**`CloudTask`**: A newly created `CloudTask` object.

#### Example:
```csharp
CloudTask task = CreateTask(
    taskId: "Task-123",
    commandLine: "cmd /c 'echo Hello World'",
    environmentSettings: new List<EnvironmentSetting>
    {
        new EnvironmentSetting("VAR_NAME", "VAR_VALUE")
    },
    dependsOnTaskIds: new List<string> { "Task-1", "Task-2" }
);
```
---

<a name="utilities-command-line"></a>
# Command Line Utilities
* [GetInstalledApplicationPath](#utilities-command-line-get-installed-application-path)
---

<a name="utilities-command-line-get-installed-application-path"></a>
### `GetInstalledApplicationPath(bool isWindows, string appName, string appVersion, string? relativePath = null)`

Returns the path for an installed application in the pool node.

#### Parameters:
- **`bool isWindows`**: Set to `true` if the virtual machine has Windows installed.
- **`string appName`**: Application name as it is provided in pool applications.
- **`string appVersion`**: Application version as it is provided in pool applications.
- **`string? relativePath`** *(optional)*: Relative path to the executable within the uploaded application package.

#### Returns:
**`string`**: Path to the installed application in the pool node.

#### Remarks:
- For additional details, see the [Azure Batch Application Packages documentation](https://learn.microsoft.com/en-us/azure/batch/batch-application-packages).

#### Example:
```csharp
string appPath = GetInstalledApplicationPath(
    isWindows: true,
    appName: "MyApp",
    appVersion: "1.0",
    relativePath: @"bin\MyApp.exe"
);
```
---

<a name="utilities-virtual-machine"></a>
# Virtual Machine Utilities
* [MatchVirtualMachineConfiguration](#utilities-virtual-machine-match-virtual-machine-configuration)
---

<a name="utilities-virtual-machine-match-virtual-machine-configuration"></a>
### `MatchVirtualMachineConfiguration(List<ImageInformation> images, string? sku = null, string? offer = null, string? publisher = null)`

Returns a `VirtualMachineConfiguration` from the provided images based on the given parameters.

#### Parameters:
- **`List<ImageInformation> images`**: List of image information.
- **`string? sku`** *(optional)*: The highest priority when choosing an image.
- **`string? offer`** *(optional)*: The second highest priority when choosing an image.
- **`string? publisher`** *(optional)*: The least priority when choosing an image.

#### Returns:
**`VirtualMachineConfiguration`**: The matched `VirtualMachineConfiguration`.

#### Exceptions:
- **`ArgumentException`**: Thrown when no image matches the provided parameters.

#### Remarks:
- At least one of the three optional parameters must be provided.
- If all three optional parameters are provided, the method first looks for the image that matches all conditions.
- If no match is found or not all optional parameters are provided, the SKU takes the highest priority, followed by the offer and the publisher.

#### Example:
```csharp
VirtualMachineConfiguration vmConfig = MatchVirtualMachineConfiguration(
    images: imageList,
    sku: "Standard_D2_v3",
    offer: "OfferName",
    publisher: "PublisherName"
);
```
---