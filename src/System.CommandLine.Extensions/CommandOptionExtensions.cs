namespace System.CommandLine.Extensions
{
    using Diagnostics.CodeAnalysis;
    using Threading.Tasks;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static partial class CommandOptionExtensions
    {
        public static CommandOption Option<T>(this CommandOption commandOption, string template, string? description = null, ArgumentArity argumentArity = default)
        {
            ArgumentNullException.ThrowIfNull(commandOption);

            return commandOption.Command.Option<T>(template, description, argumentArity);
        }

        public static void OnExecute(this CommandOption commandOption, Func<Task<int>> execute) => commandOption.Command.OnExecute(execute);
    }
}