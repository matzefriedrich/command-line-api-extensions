namespace System.CommandLine.Extensions
{
    using Binding;
    using Collections.Generic;
    using Diagnostics.CodeAnalysis;
    using IO;
    using Linq;
    using Parsing;
    using Threading.Tasks;
    
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public partial class CommandLineApplication
    {
        private readonly Command command;

        public CommandLineApplication() : this(new RootCommand())
        {
        }

        private CommandLineApplication(Command command) => this.command = command ?? throw new ArgumentNullException(nameof(command));

        public CommandLineApplication Command(string name, string? description = null, Action<CommandLineApplication>? factory = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            var subCommand = new Command(name, description);
            this.command.AddCommand(subCommand);
            var wrapper = new CommandLineApplication(subCommand);
            factory?.Invoke(wrapper);
            return this;
        }

        public CommandOption Option<T>(string template, string? description = null, ArgumentArity argumentArity = default)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(template));

            string[] aliases = template.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            Type argumentType = typeof(T);
            var option = new Option<T>(aliases, description)
            {
                Arity = argumentArity,
                IsRequired = Nullable.GetUnderlyingType(argumentType) == null
            };
            
            this.command.AddOption(option);
            return new CommandOption(this, option);
        }
        
        public void OnExecute(Func<Task<int>> execute) => this.command.SetHandler(execute);

        private IValueDescriptor<T> FindValueDescriptorForOption<T>(int index)
        {
            IReadOnlyList<Option> options = this.command.Options.ToList();
            if (index >= 0 && index < options.Count)
            {
                return (Option<T>) options[index];
            }

            return new Option<T>($"missing{index}");
        }
        
        public CommandLineApplication Argument<T>(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var argument = new Argument<T>(name) {Description = description};
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

            var commandLineConfiguration = new CommandLineConfiguration(app.command);

            var p = new Parser(commandLineConfiguration);
            return await p.InvokeAsync(command, new SystemConsole());
        }
    }
}