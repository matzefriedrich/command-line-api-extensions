namespace System.CommandLine.Extensions
{
    using Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static class CommandLineApplicationExtensions
    {
        public static CommandLineApplication Command(this CommandLineApplication application, string name, Action<CommandLineApplication>? factory = null) => application.Command(name, null, factory);
    }
}