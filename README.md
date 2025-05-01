# MBatch.Azure.Extensions

This is a nuget package for extending native client capabilities by extensions methods to interact with Azure Batch Account. It also contains a few static utilities methods.
Internally Azure.ResourceManager.Batch and Microsoft.Azure.Batch are consumed as way of comunicating with Azure Batch Account.

Following classes are extended:
* `ArmClient`
* `BatchAccountResource`
* `BatchClient`
* `CloudJob`
* `CloudPool`

## Prerequisites
Enough access rights to Azure Subscription.

The resource to fully use extensions methods from this package needs to have following permissions within Azure Subscription/Batch Account:
* read List or Get Batch Accounts
* read List or Get Applications
* read Get Application Package
* read List or Get Pools
* write Create or Update Pool
* other Stop Pool Resize
* read List Supported Batch Virtual Machinde VM

additionally for deploying application for a Batch Account:
* write Create or Update Applciation Package
* other Activate Application Package

It is possible to create custom role(s) within a Azure Subscription and assign above permission to it.

## Documentation
[Documentation](Docs/Main.md)

## Troubleshooting
[Github issues](https://github.com/marcin-gutka/MBatch/issues)
 