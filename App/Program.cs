using fvm.Commands;
using Spectre.Console;

namespace fvm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var commands = new List<ICommand>
            {
                new NewCommand(),
                new EditCommand(),
                new DeleteCommand(),
                new ListCommand()
            };

            commands.Add(new HelpCommand(commands));

            AnsiConsole.Write(new FigletText("FVM").Color(Color.Green));
            AnsiConsole.MarkupLine("[grey]FiveM Resource Creator[/]\n");

            string commandName;
            string[] commandArgs;

            if (args.Length == 0)
            {
                var choices = commands.Select(c => c.Name).ToList();
                commandName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Select a [yellow]command[/]:")
                    .AddChoices(choices));
                commandArgs = Array.Empty<string>();
            }
            else
            {
                commandName = args[0].ToLowerInvariant();
                commandArgs = args.Skip(1).ToArray();
            }

            var command = commands.FirstOrDefault(c =>
                c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                AnsiConsole.MarkupLine($"[red]Unknown command: {commandName}[/]");
                AnsiConsole.MarkupLine("[grey]Use 'fvm help' to see available commands.[/]");
                return;
            }

            try
            {
                command.Execute(commandArgs);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
#if DEBUG
                AnsiConsole.WriteException(ex);
#endif
            }
        }
    }
}
