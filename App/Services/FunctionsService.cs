using Spectre.Console;

namespace fvm.Services
{
    public static class FunctionsService
    {
        public static void CopyFunctions(string resourcePath, string? functionsFileName)
        {
            if (string.IsNullOrEmpty(functionsFileName))
                return;

            var functionsDir = PathService.GetFunctionsPath();

            var serverSource = Path.Combine(functionsDir, "server", $"functions_{functionsFileName}.lua");
            var clientSource = Path.Combine(functionsDir, "client", $"functions_{functionsFileName}.lua");

            var serverDest = Path.Combine(resourcePath, "server", "functions.lua");
            var clientDest = Path.Combine(resourcePath, "client", "functions.lua");

            Directory.CreateDirectory(Path.GetDirectoryName(serverDest)!);
            Directory.CreateDirectory(Path.GetDirectoryName(clientDest)!);

            if (File.Exists(serverSource))
                File.Copy(serverSource, serverDest, overwrite: true);
            else
                AnsiConsole.MarkupLine($"[red]⚠[/] Missing server source file: [grey]{serverSource}[/]");

            if (File.Exists(clientSource))
                File.Copy(clientSource, clientDest, overwrite: true);
            else
                AnsiConsole.MarkupLine($"[red]⚠[/] Missing client source file: [grey]{clientSource}[/]");

            AnsiConsole.MarkupLine($"[green]✓[/] Added [yellow]{functionsFileName}[/] functions.");
        }

        public static string? DetermineFunctionsFile(string resourceBase, List<string> snippets, bool includeFunctions)
        {
            if (!includeFunctions)
                return null;

            if (resourceBase.Equals("lsModule", StringComparison.OrdinalIgnoreCase) ||
                resourceBase.Equals("lsResource", StringComparison.OrdinalIgnoreCase))
                return null;

            if (snippets.Contains("Logger-system"))
                return "LoggerSystem";

            if (resourceBase.Equals("mCore", StringComparison.OrdinalIgnoreCase))
                return "mCore";

            return "Native";
        }
    }
}
