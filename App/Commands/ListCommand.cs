using fvm.Services;
using Spectre.Console;

namespace fvm.Commands
{
    public class ListCommand : ICommand
    {
        public string Name => "list";
        public string Description => "List FiveM resources in current directory";

        public void Execute(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var directories = FileService.GetDirectories(currentDir);

            if (directories.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No folders found in current directory.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Resource");
            table.AddColumn("Base");
            table.AddColumn("Frontend");
            table.AddColumn("Author");
            table.Border(TableBorder.Rounded);

            int fvmResourceCount = 0;

            foreach (var dir in directories)
            {
                var resourcePath = Path.Combine(currentDir, dir);
                var config = ConfigService.LoadConfig(resourcePath);

                if (config != null)
                {
                    fvmResourceCount++;
                    table.AddRow(
                        $"[cyan]{dir}[/]",
                        config.BaseResource,
                        config.Frontend,
                        config.Author
                    );
                }
                else if (File.Exists(Path.Combine(resourcePath, "fxmanifest.lua")))
                {
                    table.AddRow(
                        $"[grey]{dir}[/]",
                        "[grey]unknown[/]",
                        "[grey]-[/]",
                        "[grey]-[/]"
                    );
                }
            }

            if (table.Rows.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No FiveM resources found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[bold]FiveM Resources[/] ({fvmResourceCount} managed by fvm)\n");
            AnsiConsole.Write(table);
        }
    }
}
