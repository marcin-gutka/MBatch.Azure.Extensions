namespace MBatch.Extensions
{
    public static class CommandLineUtilities
    {
        //https://learn.microsoft.com/en-us/azure/batch/batch-application-packages
        public static string GetInstalledApplicationPath(string appName, string appVersion, string relativePath, bool isWindows)
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

        public static string CreateCmd(string command) =>
            $"cmd /c {command}";
    }
}
