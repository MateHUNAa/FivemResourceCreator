using fvm.Services;
using Spectre.Console;

namespace fvm.Commands
{
    public class DeleteCommand : ICommand
    {
        public string Name => "del";
        public string Description => "Delete a FiveM resource";

        public void Execute(string[] args)
        {
            AnsiConsole.MarkupLine("[bold red]Delete resource...[/]\n");

            var resourceName = SelectResource();
            if (string.IsNullOrEmpty(resourceName))
                return;

            var resourcePath = PathService.GetResourcePath(resourceName);

            ShowResourceInfo(resourcePath, resourceName);

            var deleteMode = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("What would you like to delete?")
                .AddChoices(
                    "Entire resource",
                    "Specific files/folders",
                    "Cancel"
                ));

            switch (deleteMode)
            {
                case "Entire resource":
                    DeleteEntireResource(resourcePath, resourceName);
                    break;
                case "Specific files/folders":
                    DeleteSpecificFiles(resourcePath);
                    break;
                case "Cancel":
                    AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
                    break;
            }
        }

        private static string? SelectResource()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var directories = FileService.GetDirectories(currentDir)
                .Where(d => Directory.Exists(Path.Combine(currentDir, d)))
                .ToList();

            if (directories.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No resources found in current directory.[/]");
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
                .Title("Select a [yellow]resource[/] to delete:")
                .AddChoices(directories));
        }

        private static void ShowResourceInfo(string resourcePath, string resourceName)
        {
            var files = FileService.GetAllFiles(resourcePath);
            var config = ConfigService.LoadConfig(resourcePath);

            var panel = new Panel(
                $"[yellow]Name:[/] {resourceName}\n" +
                $"[yellow]Path:[/] {resourcePath}\n" +
                $"[yellow]Files:[/] {files.Count}\n" +
                (config != null ? $"[yellow]Base:[/] {config.BaseResource}" : "[grey]No template.json found[/]"))
            {
                Header = new PanelHeader("Resource Info")
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        private static void DeleteEntireResource(string resourcePath, string resourceName)
        {
            var confirm = AnsiConsole.Confirm($"[red]Are you sure you want to delete '{resourceName}'? This cannot be undone![/]", false);

            if (!confirm)
            {
                AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
                return;
            }

            var doubleConfirm = AnsiConsole.Ask<string>($"Type [red]{resourceName}[/] to confirm deletion:", "");

            if (!doubleConfirm.Equals(resourceName, StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine("[grey]Names don't match. Operation cancelled.[/]");
                return;
            }

            try
            {
                FileService.DeleteDirectory(resourcePath);
                AnsiConsole.MarkupLine($"[green]✓[/] Resource [yellow]'{resourceName}'[/] deleted.");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error deleting resource: {ex.Message}[/]");
            }
        }

        private static void DeleteSpecificFiles(string resourcePath)
        {
            var allFiles = FileService.GetAllFiles(resourcePath);
            var allDirs = FileService.GetDirectories(resourcePath);

            var choices = allDirs.Select(d => $"[folder] {d}/").Concat(allFiles.Take(50)).ToList();

            if (choices.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No files found.[/]");
                return;
            }

            if (allFiles.Count > 50)
            {
                AnsiConsole.MarkupLine($"[grey]Showing first 50 of {allFiles.Count} files.[/]");
            }

            var selected = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                .Title("Select [yellow]files/folders[/] to delete:")
                .NotRequired()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                .AddChoices(choices));

            if (selected.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No files selected.[/]");
                return;
            }

            var confirm = AnsiConsole.Confirm($"[yellow]Delete {selected.Count} item(s)?[/]", false);

            if (!confirm)
            {
                AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
                return;
            }

            int deleted = 0;
            foreach (var item in selected)
            {
                try
                {
                    if (item.StartsWith("[folder] "))
                    {
                        var folderName = item.Replace("[folder] ", "").TrimEnd('/');
                        FileService.DeleteDirectory(Path.Combine(resourcePath, folderName));
                    }
                    else
                    {
                        FileService.DeleteFile(Path.Combine(resourcePath, item));
                    }
                    deleted++;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error deleting {item}: {ex.Message}[/]");
                }
            }

            AnsiConsole.MarkupLine($"[green]✓[/] Deleted {deleted} item(s).");
        }
    }
}
