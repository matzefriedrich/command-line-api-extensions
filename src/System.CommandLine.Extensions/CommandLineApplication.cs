namespace System.CommandLine.Extensions
{
    using Invocation;
    using IO;
    using Parsing;
    using Threading.Tasks;

    public class CommandLineApplication
    {
        private readonly Command command;

        public CommandLineApplication() : this(new RootCommand())
        {
        }

        private CommandLineApplication(Command command) => this.command = command ?? throw new ArgumentNullException(nameof(command));

        public CommandLineApplication Command(string name, string description = null, Action<CommandLineApplication> factory = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            var command = new Command(name, description);
            this.command.AddCommand(command);
            var wrapper = new CommandLineApplication(command);
            factory?.Invoke(wrapper);
            return this;
        }

        public CommandOption Option<T>(string template, string description = null, IArgumentArity argumentArity = null)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(template));

            string[] aliases = template.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            Type argumentType = typeof(T);
            var option = new Option(aliases, description, argumentType, arity: argumentArity)
            {
                IsRequired = Nullable.GetUnderlyingType(argumentType) == null
            };

            this.command.AddOption(option);
            return new CommandOption(this, option);
        }

        public void OnExecute(Func<Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1>(Func<T1, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2>(Func<T1, T2, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3>(Func<T1, T2, T3, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);
        public void OnExecute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<int>> execute) => this.command.Handler = CommandHandler.Create(execute);

        public CommandLineApplication Argument<T>(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var argument = new Argument<string>(name) {Description = description};
            this.command.Add(argument);
            return this;
        }

        public CommandLineApplication Help(string description)
        {
            this.command.Description = description;
            return this;
        }

        public int Execute(params string[] args) => this.command.Invoke(args);
        public async Task<int> ExecuteAsync(params string[] args) => await this.command.InvokeAsync(args);

        public static async Task<int> ConfigureAndExecuteAsync(ICommandsConfiguration configuration, string command)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var app = new CommandLineApplication();
            configuration.Configure(app);

            var commandLineConfiguration = new CommandLineConfiguration(new[]
            {
                app.command
            });

            var p = new Parser(commandLineConfiguration);
            return await p.InvokeAsync(command, new SystemConsole());
        }
    }
}