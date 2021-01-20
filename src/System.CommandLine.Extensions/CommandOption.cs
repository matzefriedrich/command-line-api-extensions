namespace System.CommandLine.Extensions
{
    using Invocation;
    using Threading.Tasks;

    public sealed class CommandOption
    {
        private readonly Command command;

        public CommandOption(Command command) => this.command = command ?? throw new ArgumentNullException(nameof(command));

        public CommandOption Command(string name, string description, Action<CommandOption> factory = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            var local = new Command(name, description);
            this.command.AddCommand(local);
            var wrapper = new CommandOption(local);
            factory?.Invoke(wrapper);
            return this;
        }

        public CommandOption Option<T>(string template, string description = null, IArgumentArity arity = null)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(template));

            string[] aliases = template.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            var option = new Option(aliases, description)
            {
                Argument = new Argument<T> {Arity = arity},
                IsRequired = Nullable.GetUnderlyingType(typeof(T)) == null
            };

            this.command.AddOption(option);
            return this;
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

        public CommandOption Argument<T>(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var argument = new Argument<string>(name) {Description = description};
            this.command.Add(argument);
            return this;
        }

        public CommandOption Help(string description)
        {
            this.command.Description = description;
            return this;
        }
    }
}