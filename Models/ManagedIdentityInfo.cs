namespace MBatch.Azure.Extensions.Models
{
    /// <summary>
    /// Model to provide pool managed identity.
    /// </summary>
    /// <param name="SubscriptionId">The subscription ID within which the Managed Identity is located.</param>
    /// <param name="ResourceGroup">The resource group name within which the Managed Identity is located.</param>
    /// <param name="IdentityName">Managed Identity name.</param>
    public record ManagedIdentityInfo(string SubscriptionId, string ResourceGroup, string IdentityName);
}
