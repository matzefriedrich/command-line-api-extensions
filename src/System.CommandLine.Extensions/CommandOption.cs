namespace System.CommandLine.Extensions
{
    using Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class CommandOption
    {
        internal readonly CommandLineApplication Command;
        private readonly Option option;

        internal CommandOption(CommandLineApplication command, Option option)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.option = option ?? throw new ArgumentNullException(nameof(option));
        }

        public CommandOption Suggest(params string[] completions)
        {
            this.option.AddCompletions(completions);

            return this;
        }
    }
}