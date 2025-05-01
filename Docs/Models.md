<a name="models"></a>
# Models
* [IScaleSettings](#models-iscale-settings)
* [FixedScaleSettings](#models-fixed-scale-settings)
* [ManagedIdentityInfo](#models-managed-identity-info)
* [StartTaskSettings](#models-start-task-settings)
---

<a name="models-iscale-settings"></a>
### `IScaleSettings`

Interface for setting pool scale settings.

#### Remarks:
- Currently, only fixed scale is supported.
---

<a name="models-fixed-scale-settings"></a>
### `FixedScaleSettings(int TargetDedicatedNodes)`

Model to provide pool fixed scale settings. Implements [`IScaleSettings`](#models-iscale-settings)

#### Parameters:
- **`int TargetDedicatedNodes`**: Sets target dedicated nodes.

#### Remarks:
- Low-priority nodes are not currently supported by this extension.

#### Example:
```csharp
FixedScaleSettings fixedScaleSettings = new FixedScaleSettings(2)
```
---

<a name="models-managed-identity-info"></a>
### `ManagedIdentityInfo(string SubscriptionId, string ResourceGroup, string IdentityName)`

Model to provide pool managed identity.

#### Parameters:
- **`string SubscriptionId`**: The subscription ID within which the Managed Identity is located.
- **`string ResourceGroup`**: The resource group name within which the Managed Identity is located.
- **`string IdentityName`**: Managed Identity name.

#### Example:
```csharp
ManagedIdentityInfo identityInfo = new ManagedIdentityInfo(
    "abc123-456def-789ghi",
    "MyResourceGroup",
    "MyManagedIdentity"
);
```
---

<a name="models-start-task-settings"></a>
### `StartTaskSettings(string CommandLine, bool WaitForSuccess, BatchUserAccountElevationLevel ElevationLevel, BatchAutoUserScope AutoUserScope)`

Model to provide pool start task.

#### Parameters:
- **`string CommandLine`**: Command line for the start task.
- **`bool WaitForSuccess`**: When set to `true`, the pool remains in `ComputeNodeState.WaitingForStartTask` until the start task is completed.
- **`BatchUserAccountElevationLevel ElevationLevel`**: Sets privileges to run the start task.
- **`BatchAutoUserScope AutoUserScope`**: Configures task execution for a new user specified for the start task or a general auto-user account across each node.

#### Example:
```csharp
StartTaskSettings startTask = new StartTaskSettings(
    CommandLine: "cmd /c 'echo Hello World'",
    true,
    BatchUserAccountElevationLevel.Admin,
    BatchAutoUserScope.Task
);
```