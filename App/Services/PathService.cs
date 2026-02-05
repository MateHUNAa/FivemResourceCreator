namespace fvm.Services
{
    public static class PathService
    {
        private static string? cachedProjectRoot;

        public static string GetProjectRoot()
        {
            if (cachedProjectRoot != null)
                return cachedProjectRoot;

            var dir = AppContext.BaseDirectory;

            while (dir != null && !Directory.Exists(Path.Combine(dir, "Templates")))
                dir = Directory.GetParent(dir)?.FullName;

            if (dir == null)
                throw new DirectoryNotFoundException("Could not locate project root containing 'Templates'.");

            cachedProjectRoot = dir;
            return dir;
        }

        public static string GetResourcePath(string resourceName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), resourceName);
        }

        public static string GetTemplatePath(string resourceBase)
        {
            return Path.Combine(GetProjectRoot(), "Templates", resourceBase);
        }

        public static string GetCommonEditorPath()
        {
            return Path.Combine(GetProjectRoot(), "CommonEditor");
        }

        public static string GetFrontendPath(string frontend)
        {
            return Path.Combine(GetProjectRoot(), "FrontendTemplates", frontend);
        }

        public static string GetFunctionsPath()
        {
            return Path.Combine(GetProjectRoot(), "Functions");
        }

        public static bool ResourceExists(string resourceName)
        {
            return Directory.Exists(GetResourcePath(resourceName));
        }

        public static bool HasTemplateJson(string resourcePath)
        {
            return File.Exists(Path.Combine(resourcePath, "template.json"));
        }
    }
}
