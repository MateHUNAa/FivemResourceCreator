using fvm.Models;
using fvm.Services;
using Spectre.Console;

namespace fvm.Commands
{
    public class NewCommand : ICommand
    {
        public string Name => "new";
        public string Description => "Create a new FiveM resource";

        private static readonly string[] BaseChoices = { "mCore", "ESX", "Standalone", "lsModule", "lsResource" };
        private static readonly string[] FrontendChoices = { "None", "React" };

        public void Execute(string[] args)
        {
            AnsiConsole.MarkupLine("[bold cyan]Creating new resource...[/]\n");

            var resourceName = PromptResourceName();
            if (string.IsNullOrEmpty(resourceName))
                return;

            var resourceBase = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose a [yellow]base resource[/]:")
                .AddChoices(BaseChoices));

            var frontend = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose a [yellow]Front-end framework[/]:")
                .AddChoices(FrontendChoices));

            var snippets = PromptSnippets(resourceBase, frontend);

            var includeFunctions = AnsiConsole.Confirm("Include [yellow]common utility functions[/]?", true);
            var functionsFileName = FunctionsService.DetermineFunctionsFile(resourceBase, snippets, includeFunctions);

            var author = AnsiConsole.Ask<string>("Author:", "MateHUN");
            var description = AnsiConsole.Ask<string>("Description:", "A new FiveM resource");

            var config = new ResourceConfig
            {
                Name = resourceName,
                BaseResource = resourceBase,
                Frontend = frontend,
                Snippets = snippets,
                Author = author,
                Description = description,
                FunctionsFileName = functionsFileName
            };

            CreateResource(config);
        }

        private static string? PromptResourceName()
        {
            var resourceName = AnsiConsole.Ask<string>("Resource name:");

            if (string.IsNullOrWhiteSpace(resourceName))
            {
                AnsiConsole.MarkupLine("[red]Resource name cannot be empty.[/]");
                return null;
            }

            if (PathService.ResourceExists(resourceName))
            {
                var overwrite = AnsiConsole.Confirm($"[yellow]Resource '{resourceName}' already exists. Overwrite?[/]", false);
                if (!overwrite)
                {
                    AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
                    return null;
                }
                FileService.DeleteDirectory(PathService.GetResourcePath(resourceName));
            }

            return resourceName;
        }

        private static List<string> PromptSnippets(string resourceBase, string frontend)
        {
            var snippetOptions = new List<SnippetOption>
            {
                new() { Name = "Grid-system",   Tags = { } },
                new() { Name = "Logger-system", Tags = { } },
                new() { Name = "OxMySQL",       Tags = { } },
                new() { Name = "ESX",           Tags = { "esx" } },
                new() { Name = "NUI",           Tags = { "react" } },
                new() { Name = "ClientLoader",  Tags = { } },
                new() { Name = "Repository",    Tags = { "ls" } },
                new() { Name = "DomainEntity",  Tags = { "ls" } },
                new() { Name = "InstanceWrapper", Tags = { "ls" } },
                new() { Name = "EventHandler",  Tags = { } },
            };

            var filteredSnippets = snippetOptions
                .Where(s => s.Matches(resourceBase, frontend))
                .Select(s => s.Name)
                .ToList();

            if (filteredSnippets.Count == 0)
                return new List<string>();

            var snippetPrompt = new MultiSelectionPrompt<string>()
                .Title("Select [yellow]code snippets[/] to include:")
                .NotRequired()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                .AddChoices(filteredSnippets);

            snippetPrompt.Select("OxMySQL");

            if (!resourceBase.Equals("lsModule", StringComparison.OrdinalIgnoreCase) &&
                !resourceBase.Equals("lsResource", StringComparison.OrdinalIgnoreCase))
                snippetPrompt.Select("Logger-system");

            if (resourceBase.Equals("ESX", StringComparison.OrdinalIgnoreCase))
                snippetPrompt.Select("ESX");

            if (!frontend.Equals("None", StringComparison.OrdinalIgnoreCase))
                snippetPrompt.Select("NUI");

            return AnsiConsole.Prompt(snippetPrompt);
        }

        private static void CreateResource(ResourceConfig config)
        {
            var resourcePath = PathService.GetResourcePath(config.Name);
            Directory.CreateDirectory(resourcePath);

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .Start("Creating resource...", ctx =>
                {
                    ctx.Status("Copying template files...");
                    FileService.CopyDirectory(PathService.GetTemplatePath(config.BaseResource), resourcePath);
                    FileService.CopyDirectory(PathService.GetCommonEditorPath(), resourcePath);

                    if (!config.Frontend.Equals("None", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Status("Setting up frontend...");
                        FileService.CopyDirectory(PathService.GetFrontendPath(config.Frontend), resourcePath);
                    }

                    ctx.Status("Adding functions...");
                    FunctionsService.CopyFunctions(resourcePath, config.FunctionsFileName);

                    ctx.Status("Injecting snippets...");
                    SnippetService.InjectSnippets(resourcePath, config.Name, config.Snippets);

                    ctx.Status("Generating manifest...");
                    ManifestService.Generate(resourcePath, config.Author, config.Description, config.Snippets, config.Frontend, config.BaseResource);

                    ctx.Status("Saving configuration...");
                    ConfigService.SaveConfig(resourcePath, config);
                });

            AnsiConsole.MarkupLine($"\n[green]✓[/] Resource [yellow]'{config.Name}'[/] created successfully!");
            AnsiConsole.MarkupLine($"[grey]Location: {resourcePath}[/]");
        }
    }
}
