namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for extensions methods for <see cref="Uri"/>.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Decomposes Blob storage Uri into container name and file path.
        /// </summary>
        /// <param name="uri">Blob storage Uri.</param>
        public static (string ContainerName, string FullFilePath) GetBlobAbsolutePathParts(this Uri uri)
        {
            var absolutePath = uri.AbsolutePath;

            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Provided Uri does not contain absolute path.");

            var indexOfLastSlash = absolutePath.LastIndexOf('/');

            var container = absolutePath[1..indexOfLastSlash];
            var fullPath = absolutePath[(indexOfLastSlash + 1)..];

            return new(container, fullPath);
        }
    }
}
