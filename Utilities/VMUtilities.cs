using Microsoft.Azure.Batch;

namespace MBatch.Utilities
{
    public static class VMUtilities
    {
        public static VirtualMachineConfiguration MatchVirtualMachineConfiguration(List<ImageInformation> images, string? sku, string? offer, string? publisher)
        {
            var selectedImg = images.FirstOrDefault(img => img.ImageReference.Publisher == publisher &&
                    img.ImageReference.Offer == offer &&
                    img.ImageReference.Sku == sku);

            selectedImg ??= images.FirstOrDefault(img => img.ImageReference.Sku == sku);

            selectedImg ??= images.FirstOrDefault(img => img.ImageReference.Offer == offer);

            selectedImg ??= images.FirstOrDefault(img => img.ImageReference.Publisher == publisher);

            if (selectedImg is null)
                throw new ArgumentException($"Chosen virtual machine configuration '{offer}:{sku}' is not available.");
            return new(selectedImg.ImageReference, selectedImg.NodeAgentSkuId);
        }
    }
}
