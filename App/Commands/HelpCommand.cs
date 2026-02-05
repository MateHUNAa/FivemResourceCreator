using Spectre.Console;

namespace fvm.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly List<ICommand> commands;

        public string Name => "help";
        public string Description => "Show available commands";

        public HelpCommand(List<ICommand> commands)
        {
            this.commands = commands;
        }

        public void Execute(string[] args)
        {
            AnsiConsole.MarkupLine("[bold green]FiveM Resource Creator[/] - Available Commands\n");

            var table = new Table();
            table.AddColumn("Command");
            table.AddColumn("Description");
            table.Border(TableBorder.Rounded);

            foreach (var cmd in commands.OrderBy(c => c.Name))
            {
                table.AddRow($"[cyan]{cmd.Name}[/]", cmd.Description);
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Usage: fvm <command> [options][/]");
            AnsiConsole.MarkupLine("[grey]Example: fvm new[/]");
        }
    }
}
