namespace System.CommandLine.Extensions
{
    public sealed class CommandOption
    {
        internal readonly CommandLineApplication command;
        private readonly Option option;

        internal CommandOption(CommandLineApplication command, Option option)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.option = option ?? throw new ArgumentNullException(nameof(option));
        }

        public CommandOption Suggest(params string[] suggestions)
        {
            this.option.AddSuggestions(suggestions);

            return this;
        }
    }
}