namespace System.CommandLine.Extensions
{
    using Diagnostics.CodeAnalysis;
    using Threading.Tasks;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static partial class CommandOptionExtensions
    {
        public static CommandOption Option<T>(this CommandOption commandOption, string template, string? description = null)
        {
            if (commandOption == null) throw new ArgumentNullException(nameof(commandOption));


            return commandOption.Command.Option<T>(template, description);
        }

        public static void OnExecute(this CommandOption commandOption, Func<Task<int>> execute) => commandOption.Command.OnExecute(execute);
    }
}