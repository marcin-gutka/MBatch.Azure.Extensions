namespace MBatch.Azure.Extensions
{
    /// <summary>
    /// Static class for task command line utilities.
    /// </summary>
    public static class CommandLineUtilities
    {
        /// <summary>
        /// Returns path for installed application in the pool node.
        /// Check also <see href="https://learn.microsoft.com/en-us/azure/batch/batch-application-packages"/>
        /// </summary>
        /// <param name="isWindows">Set to <see langword="true"/> if virtual machine has Windows installed.</param>
        /// <param name="appName">Application name as it is provided in pool applications.</param>
        /// <param name="appVersion">Application version as it is provided in pool applications.</param>
        /// <param name="relativePath">Optional: Relative path to executable within uploaded application package.</param>
        public static string GetInstalledApplicationPath(bool isWindows, string appName, string appVersion, string? relativePath = null)
        {
            string connector;

            if (appVersion is null)
            {
                appVersion = string.Empty;
                connector = string.Empty;
            }

            if (isWindows)
            {
                appName = appName.ToUpperInvariant();
                connector = "#";
            }
            else
            {
                appName = appName.ToLowerInvariant();
                connector = "_";
            }

            var basePath = $"%AZ_BATCH_APP_PACKAGE_{appName}{connector}{appVersion}%";

            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                return $"{basePath}\\{relativePath}";
            }

            return basePath;
        }
    }
}
