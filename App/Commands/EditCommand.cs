using fvm.Models;
using fvm.Services;
using Spectre.Console;

namespace fvm.Commands
{
    public class EditCommand : ICommand
    {
        public string Name => "edit";
        public string Description => "Edit an existing FiveM resource";

        public void Execute(string[] args)
        {
            AnsiConsole.MarkupLine("[bold cyan]Edit existing resource...[/]\n");

            var resourceName = SelectResource();
            if (string.IsNullOrEmpty(resourceName))
                return;

            var resourcePath = PathService.GetResourcePath(resourceName);
            var config = ConfigService.LoadConfig(resourcePath);

            if (config == null)
            {
                AnsiConsole.MarkupLine("[red]Could not load resource configuration. This resource may not have been created with fvm.[/]");
                return;
            }

            config.Name = resourceName;

            ShowCurrentConfig(config);

            var editChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("What would you like to edit?")
                .AddChoices(
                    "Author",
                    "Description",
                    "Snippets",
                    "Regenerate Manifest",
                    "Cancel"
                ));

            switch (editChoice)
            {
                case "Author":
                    EditAuthor(resourcePath, config);
                    break;
                case "Description":
                    EditDescription(resourcePath, config);
                    break;
                case "Snippets":
                    EditSnippets(resourcePath, config);
                    break;
                case "Regenerate Manifest":
                    RegenerateManifest(resourcePath, config);
                    break;
                case "Cancel":
                    AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
                    return;
            }
        }

        private static string? SelectResource()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var directories = FileService.GetDirectories(currentDir)
                .Where(d => PathService.HasTemplateJson(Path.Combine(currentDir, d)))
                .ToList();

            if (directories.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No fvm resources found in current directory.[/]");
                var manualName = AnsiConsole.Ask<string>("Enter resource name manually (or leave empty to cancel):", "");

                if (string.IsNullOrWhiteSpace(manualName))
                    return null;

                if (!PathService.ResourceExists(manualName))
                {
                    AnsiConsole.MarkupLine($"[red]Resource '{manualName}' not found.[/]");
                    return null;
                }

                return manualName;
            }

            return AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select a [yellow]resource[/] to edit:")
                .AddChoices(directories));
        }

        private static void ShowCurrentConfig(ResourceConfig config)
        {
            var table = new Table();
            table.AddColumn("Property");
            table.AddColumn("Value");
            table.AddRow("Base Resource", config.BaseResource);
            table.AddRow("Frontend", config.Frontend);
            table.AddRow("Snippets", string.Join(", ", config.Snippets));
            table.AddRow("Author", config.Author);
            table.AddRow("Description", config.Description);

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        private static void EditAuthor(string resourcePath, ResourceConfig config)
        {
            config.Author = AnsiConsole.Ask<string>("New author:", config.Author);
            ConfigService.SaveConfig(resourcePath, config);
            ManifestService.Generate(resourcePath, config.Author, config.Description, config.Snippets, config.Frontend, config.BaseResource);
            AnsiConsole.MarkupLine("[green]✓[/] Author updated and manifest regenerated.");
        }

        private static void EditDescription(string resourcePath, ResourceConfig config)
        {
            config.Description = AnsiConsole.Ask<string>("New description:", config.Description);
            ConfigService.SaveConfig(resourcePath, config);
            ManifestService.Generate(resourcePath, config.Author, config.Description, config.Snippets, config.Frontend, config.BaseResource);
            AnsiConsole.MarkupLine("[green]✓[/] Description updated and manifest regenerated.");
        }

        private static void EditSnippets(string resourcePath, ResourceConfig config)
        {
            var snippetOptions = new List<SnippetOption>
            {
                new() { Name = "oxLib",  Tags = { } },
                new() { Name = "lsCore",  Tags = { } },
                new() { Name = "Grid-system",   Tags = { } },
                new() { Name = "Logger-system", Tags = { } },
                new() { Name = "OxMySQL",       Tags = { } },
                new() { Name = "ESX",           Tags = { "esx" } },
                new() { Name = "NUI",           Tags = { "react" } },
                new() { Name = "ClientLoader",  Tags = { } },
                new() { Name = "EventHandler",  Tags = { } },
            };

            var filteredSnippets = snippetOptions
                .Where(s => s.Matches(config.BaseResource, config.Frontend))
                .Select(s => s.Name)
                .ToList();

            var snippetPrompt = new MultiSelectionPrompt<string>()
                .Title("Select [yellow]code snippets[/]:")
                .NotRequired()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                .AddChoices(filteredSnippets);

            foreach (var existing in config.Snippets.Where(s => filteredSnippets.Contains(s)))
            {
                snippetPrompt.Select(existing);
            }

            var newSnippets = AnsiConsole.Prompt(snippetPrompt);

            var addedSnippets = newSnippets.Except(config.Snippets).ToList();
            var removedSnippets = config.Snippets.Except(newSnippets).ToList();

            if (addedSnippets.Count > 0)
            {
                SnippetService.InjectSnippets(resourcePath, config.Name, addedSnippets);
                AnsiConsole.MarkupLine($"[green]✓[/] Added snippets: {string.Join(", ", addedSnippets)}");
            }

            if (removedSnippets.Count > 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Note:[/] Removed snippets ({string.Join(", ", removedSnippets)}) may require manual cleanup.");
            }

            config.Snippets = newSnippets;
            ConfigService.SaveConfig(resourcePath, config);
            ManifestService.Generate(resourcePath, config.Author, config.Description, config.Snippets, config.Frontend, config.BaseResource);

            AnsiConsole.MarkupLine("[green]✓[/] Snippets updated and manifest regenerated.");
        }

        private static void RegenerateManifest(string resourcePath, ResourceConfig config)
        {
            ManifestService.Generate(resourcePath, config.Author, config.Description, config.Snippets, config.Frontend, config.BaseResource);
            AnsiConsole.MarkupLine("[green]✓[/] Manifest regenerated.");
        }
    }
}
