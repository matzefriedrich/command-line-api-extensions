namespace System.CommandLine.Extensions
{
    using Invocation;
    using IO;
    using Parsing;
    using Threading.Tasks;

    public class CommandLineApplication
    {
        private readonly RootCommand rootCommand = new RootCommand();

        public CommandLineApplication Command(string name, Action<CommandOption> factory = null) => this.Command(name, null, factory);

        public CommandLineApplication Command(string name, string description = null, Action<CommandOption> factory = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            var command = new Command(name, description);
            this.rootCommand.AddCommand(command);
            var wrapper = new CommandOption(command);
            factory?.Invoke(wrapper);
            return this;
        }

        public CommandlineOption Option<T>(string template, string description = null)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(template));

            string[] aliases = template.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            var option = new Option(aliases, description)
            {
                Argument = new Argument<T>(),
                IsRequired = Nullable.GetUnderlyingType(typeof(T)) == null
            };

            this.rootCommand.AddOption(option);
            return new CommandlineOption(option);
        }

        public void OnExecute(Func<Task<int>> execute) => this.rootCommand.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1>(Func<T1, Task<int>> execute) => this.rootCommand.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2>(Func<T1, T2, Task<int>> execute) => this.rootCommand.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3>(Func<T1, T2, T3, Task<int>> execute) => this.rootCommand.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task<int>> execute) => this.rootCommand.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task<int>> execute) => this.rootCommand.Handler = CommandHandler.Create(execute);

        public int Execute(params string[] args) => this.rootCommand.Invoke(args);

        public static async Task<int> ConfigureAndExecuteAsync(ICommandsConfiguration configuration, string command)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var app = new CommandLineApplication();
            configuration.Configure(app);

            var commandLineConfiguration = new CommandLineConfiguration(new[]
            {
                app.rootCommand
            });

            var p = new Parser(commandLineConfiguration);
            return await p.InvokeAsync(command, new SystemConsole());
        }
    }
}