﻿<a name="arm-client-extensions"></a>
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
    skuName: "2016-datacenter-smalldisk",
    minMemory: 4.0,
    minvCPUs: 2.0
);
```