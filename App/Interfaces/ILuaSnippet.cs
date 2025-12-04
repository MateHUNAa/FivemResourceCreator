namespace fvm.Interfaces
{
    public interface ILuaSnippet
    {
        string Name { get; }                 // Friendly name of the snippet
        string TargetFile { get; }           // File to inject into (e.g., "client/main.lua")
        string GetCode(Dictionary<string, string> placeholders); // Return Lua code with placeholders replaced
    }
}
