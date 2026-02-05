using fvm.Models;
using System.Text.Json;

namespace fvm.Services
{
    public static class ConfigService
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public static void SaveConfig(string resourcePath, ResourceConfig config)
        {
            var json = JsonSerializer.Serialize(new
            {
                baseResource = config.BaseResource,
                frontend = config.Frontend,
                snippets = config.Snippets,
                author = config.Author,
                description = config.Description
            }, JsonOptions);

            File.WriteAllText(Path.Combine(resourcePath, "template.json"), json);
        }

        public static ResourceConfig? LoadConfig(string resourcePath)
        {
            var configPath = Path.Combine(resourcePath, "template.json");
            if (!File.Exists(configPath))
                return null;

            try
            {
                var json = File.ReadAllText(configPath);
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                return new ResourceConfig
                {
                    BaseResource = root.GetProperty("baseResource").GetString() ?? "Standalone",
                    Frontend = root.GetProperty("frontend").GetString() ?? "None",
                    Snippets = root.GetProperty("snippets").EnumerateArray()
                        .Select(e => e.GetString() ?? "")
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList(),
                    Author = root.GetProperty("author").GetString() ?? "",
                    Description = root.GetProperty("description").GetString() ?? ""
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
