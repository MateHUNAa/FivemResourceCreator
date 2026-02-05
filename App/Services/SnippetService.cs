using fvm.Interfaces;

namespace fvm.Services
{
    public static class SnippetService
    {
        private static List<ILuaSnippet>? cachedSnippets;

        public static List<ILuaSnippet> GetAllSnippets()
        {
            if (cachedSnippets != null)
                return cachedSnippets;

            cachedSnippets = typeof(SnippetService).Assembly
                .GetTypes()
                .Where(t => typeof(ILuaSnippet).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (ILuaSnippet)Activator.CreateInstance(t)!)
                .ToList();

            return cachedSnippets;
        }

        public static void InjectSnippets(string resourcePath, string resourceName, List<string> selectedSnippets)
        {
            var allSnippets = GetAllSnippets();

            var placeholders = new Dictionary<string, string>
            {
                ["resourceName"] = resourceName
            };

            var snippetsToInject = allSnippets
                .Where(s => selectedSnippets.Contains(s.Name))
                .ToList();

            foreach (var snippet in snippetsToInject)
            {
                var filePath = Path.Combine(resourcePath, snippet.TargetFile);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                var code = snippet.GetCode(placeholders);

                if (File.Exists(filePath))
                    File.AppendAllText(filePath, "\n" + code);
                else
                    File.WriteAllText(filePath, code);
            }
        }

        public static void RemoveSnippetFiles(string resourcePath, List<string> snippetNames)
        {
            var allSnippets = GetAllSnippets();

            var snippetsToRemove = allSnippets
                .Where(s => snippetNames.Contains(s.Name))
                .ToList();

            foreach (var snippet in snippetsToRemove)
            {
                var filePath = Path.Combine(resourcePath, snippet.TargetFile);
                FileService.DeleteFile(filePath);
            }
        }
    }
}
