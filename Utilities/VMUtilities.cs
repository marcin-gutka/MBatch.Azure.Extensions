using Microsoft.Azure.Batch;

namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for Virtual Machines utilities.
    /// </summary>
    public static class VMUtilities
    {
        /// <summary>
        /// Returns Virtual Machine Configuration from provided images based on provided parameters.
        /// </summary>
        /// <param name="images">List of Image Information.</param>
        /// <param name="sku">Optional: The highest priority when choosing image.</param>
        /// <param name="offer">Optional: The second highest priority when choosing image.</param>
        /// <param name="publisher">Optional: The least priority when choosing image.</param>
        /// <returns>Matched <see cref="VirtualMachineConfiguration"/></returns>
        /// <exception cref="ArgumentException">When any image didn't match provided parameters.</exception>
        /// <remarks>At least one of three optional parameters needs to be provided. If all three optional parameters are provided, the method first looks for the image that matches all conditions. If no match is found, or if not all optional parameters are provided, the SKU takes the highest priority, followed by the offer and the publisher.</remarks>
        public static VirtualMachineConfiguration MatchVirtualMachineConfiguration(List<ImageInformation> images, string? sku = null, string? offer = null, string? publisher = null)
        {
            if (string.IsNullOrWhiteSpace(sku) && string.IsNullOrWhiteSpace(offer) && string.IsNullOrWhiteSpace(publisher))
                throw new ArgumentException("Any parameter was not provided. Cannot choose any image.");

            var selectedImg = images.FirstOrDefault(img => img.ImageReference.Publisher == publisher &&
                    img.ImageReference.Offer == offer &&
                    img.ImageReference.Sku == sku);

            selectedImg ??= images.FirstOrDefault(img => img.ImageReference.Sku == sku);

            selectedImg ??= images.FirstOrDefault(img => img.ImageReference.Offer == offer);

            selectedImg ??= images.FirstOrDefault(img => img.ImageReference.Publisher == publisher);

            if (selectedImg is null)
                throw new ArgumentException("Could not match any image. Try to provide more data (sku/offer/publisher).");

            return new(selectedImg.ImageReference, selectedImg.NodeAgentSkuId);
        }
    }
}
