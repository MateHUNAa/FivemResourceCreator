namespace fvm.Models
{
    public class ResourceConfig
    {
        public string Name { get; set; } = string.Empty;
        public string BaseResource { get; set; } = string.Empty;
        public string Frontend { get; set; } = "None";
        public List<string> Snippets { get; set; } = new();
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? FunctionsFileName { get; set; }
    }
}
