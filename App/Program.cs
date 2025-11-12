using App.Interfaces;
using Spectre.Console;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold green] Fivem Resource Creator [/]");

            var resourceName = AnsiConsole.Ask<string>("Resource name:");

            var resourceBase = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose a [yellow]base resource[/]:")
                .AddChoices("mCore", "ESX", "Standalone"));
            
            var frontend = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose a [yellow]Front-end framework[/]:")
                .AddChoices("None", "React"));

            var snippetPropmt = new MultiSelectionPrompt<string>()
                    .Title("Select [yellow]code snippets[/] to include:")
                    .NotRequired()
                    .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                    .AddChoices("Grid-system", "Logger-system", "OxMySQL", "ESX", "NUI", "ClientLoader");


            snippetPropmt.Select("Logger-system");
            snippetPropmt.Select("OxMySQL");


            if (resourceBase.Equals("ESX", StringComparison.OrdinalIgnoreCase))
                snippetPropmt.Select("ESX");

            if (!frontend.Equals("None", StringComparison.OrdinalIgnoreCase))
                snippetPropmt.Select("NUI");


            var snippets = AnsiConsole.Prompt(snippetPropmt);

            var includeFunctions = AnsiConsole.Confirm("Include [yellow]common utility functions[/]?", true);

            bool loggerSystemIncluded = snippets.Contains("Logger-system");
            string functionsFileName;

            if (!includeFunctions)
            {
                functionsFileName = null;
            }
            else if (loggerSystemIncluded)
            {
                functionsFileName = "LoggerSystem";
            }
            else if (resourceBase.Equals("mCore", StringComparison.OrdinalIgnoreCase))
            {
                functionsFileName = "mCore";
            }
            else
            {
                functionsFileName = "Native";
            }

            var author = AnsiConsole.Ask<string>("Author:", "MateHUN");
            var description = AnsiConsole.Ask<string>("Description:", "A new Fivem resource");

            var resourcePath = Path.Combine(Directory.GetCurrentDirectory(), resourceName);
            Directory.CreateDirectory(resourcePath);

            var projectRoot = GetProjectRoot();

            var templatePath = Path.Combine(projectRoot, "Templates", resourceBase);
            var commonEditorPath = Path.Combine(projectRoot, "CommonEditor");

            var frontendPath = Path.Combine(projectRoot, "FrontendTemplates", frontend);

            CopyDirectory(templatePath, resourcePath);
            CopyDirectory(commonEditorPath, resourcePath);
            
            if (!frontend.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                CopyDirectory(frontendPath, resourcePath);
            }

            if (!string.IsNullOrEmpty(functionsFileName))
            {
                var functionsDir = Path.Combine(projectRoot, "Functions");

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

                AnsiConsole.MarkupLine($"[green]✓[/] Added [yellow]{functionsFileName}[/] functions for [cyan]server[/] and [cyan]client[/].");
            }

            InjectSnippets(resourcePath, resourceName, snippets);

            GenerateFxManifest(resourcePath, author, description, snippets, frontend, resourceBase);

            GenerateTemplateJson(resourcePath, resourceBase, frontend, snippets, author, description);

            AnsiConsole.MarkupLine($"[green]Resource '{resourceName}' created successfully![/]");
        }

        static void InjectSnippets(string resourcePath, string resourceName, List<string> selectedSnippets)
        {
            var allSnippets = typeof(Program).Assembly
                .GetTypes()
                .Where(t => typeof(ILuaSnippet).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (ILuaSnippet)Activator.CreateInstance(t)!)
                .ToList();

            var placeholders = new Dictionary<string, string>
            {
                ["resourceName"] = resourceName
            };

            // Filter only the snippets that are selected by the user
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
        static string GetProjectRoot()
        {
            var dir = AppContext.BaseDirectory;

            while (dir != null && !Directory.Exists(Path.Combine(dir, "Templates")))
                dir = Directory.GetParent(dir)?.FullName;

            if (dir == null)
                throw new DirectoryNotFoundException("Could not locate project root containing 'Tempaltes'.");

            return dir;
        }

        static void CopyDirectory(string sourceDir, string targetDir)
        {
            foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(sourceDir, targetDir));

            foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(sourceDir, targetDir), true);
        }

        static void GenerateFxManifest(string resourcePath, string author, string description, List<string> snippets, string frontend, string resourceBase)
        {
            var lines = new List<string>
    {
        "--- @diagnostic disable: undefined-global",
        "",
        "fx_version 'cerulean'",
        "game 'gta5'",
        "",
        "lua54 'yes'",
        "",
        $"author '{author}'",
        $"description '{description}'",
        ""
    };

            var shared = new List<string>();
            var client = new List<string>();
            var server = new List<string>();
            var files = new List<string>();

            foreach (var snip in snippets)
            {
                switch (snip)
                {
                    case "Logger-system": shared.Add("'@mate-logger/init.lua'"); break;
                    case "Grid-system": client.Add("'@mate-grid/init.lua'"); break;
                    case "OxMySQL": server.Add("'@oxmysql/lib/MySQL.lua'"); break;
                    case "ClientLoader": lines.Add("shared_scrips {'@clientloader/shared.lua'}"); break;
                    case "NUI":
                        files.Add("'html/index.html'");
                        files.Add("'html/assets/*.js'");
                        files.Add("'html/assets/*.css'");
                        files.Add("'html/assets/images/{*.png, *.jpg, *.svg, *.webp, *.ico}'");
                        lines.Add("--ui_page 'html/index.html'");
                        lines.Add("ui_page 'http://localhost:5173'");
                        break;
                }
            }


            bool isCloader = snippets.Contains("ClientLoader");
          
            switch (resourceBase)
            {
                case "mCore":
                    if (isCloader)
                    {
                        lines.Add("clientloader {'client/init.lua','client/main.lua'}");
                    } else
                    {
                        client.Add("'client/init.lua'");
                        client.Add("'client/main.lua'");
                    }
                    server.Add("'server/init.lua'");
                    server.Add("'server/main.lua'");
                    shared.Add("'shared/**.*'");
                    break;
                case "ESX":
                case "Standalone":
                    if (isCloader)
                    {
                        lines.Add("clientloader {'client/init.lua','client/main.lua'}");
                    }
                    else
                    {
                        client.Add("'client/init.lua'");
                        client.Add("'client/main.lua'");
                    }
                    shared.Add("'shared/**.*'");
                    break;
            }

            // Helper function for sections
            void AddSection(string name, List<string> items)
            {
                if (items.Count == 0) return;
                lines.Add($"{name} {{");
                foreach (var item in items)
                {
                    lines.Add($"    {item},");
                }
                // Remove last comma
                if (lines.Count > 0) lines[lines.Count - 1] = lines.Last().TrimEnd(',');
                lines.Add("}");
                lines.Add(""); // blank line
            }

            AddSection("shared_scripts", shared);
            AddSection("client_scripts", client);
            AddSection("server_scripts", server);
            AddSection("files", files);

            File.WriteAllLines(Path.Combine(resourcePath, "fxmanifest.lua"), lines);
        }

        static void GenerateTemplateJson(string resourcePath, string baseResource, string frontend, List<string> snippets, string author, string description)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                baseResource,
                frontend,
                snippets,
                author,
                description
            }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true});

            File.WriteAllText(Path.Combine(resourcePath, "template.json"), json);
        }
    }
}
