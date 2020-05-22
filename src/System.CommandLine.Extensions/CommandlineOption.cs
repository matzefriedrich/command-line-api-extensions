namespace System.CommandLine.Extensions
{
    public sealed class CommandlineOption
    {
        private readonly Option option;

        public CommandlineOption(Option option)
        {
            this.option = option ?? throw new ArgumentNullException(nameof(option));
        }
    }
}