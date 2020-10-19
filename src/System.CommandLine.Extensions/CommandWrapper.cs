namespace System.CommandLine.Extensions
{
    using Invocation;
    using Threading.Tasks;

    public sealed class CommandWrapper
    {
        private readonly Command command;

        public CommandWrapper(Command command) => this.command = command ?? throw new ArgumentNullException(nameof(command));

        public CommandWrapper Command(string name, string description, Action<CommandWrapper> factory = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            var local = new Command(name, description);
            this.command.AddCommand(local);
            var wrapper = new CommandWrapper(local);
            factory?.Invoke(wrapper);
            return this;
        }

        public CommandWrapper Option<T>(string template, string description = null, IArgumentArity arity = null)
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

        public CommandWrapper Argument<T>(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var argument = new Argument<string>(name) {Description = description};
            this.command.Add(argument);
            return this;
        }
    }
}