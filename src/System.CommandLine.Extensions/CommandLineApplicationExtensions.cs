namespace System.CommandLine.Extensions
{
    public static class CommandLineApplicationExtensions
    {
        public static CommandLineApplication Command(this CommandLineApplication application, string name, Action<CommandLineApplication> factory = null) => application.Command(name, null, factory);
    }
}