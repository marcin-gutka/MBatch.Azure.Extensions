<a name="utilities"></a>
# Utilities
* [Cloud Task](#utilities-cloud-task)
* [CommandLine](#utilities-command-line)
* [VirtualMachine](#utilities-virtual-machine)
---

<a name="utilities-cloud-task"></a>
## Cloud Task Utilities
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
## Command Line Utilities
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
## Virtual Machine Utilities
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