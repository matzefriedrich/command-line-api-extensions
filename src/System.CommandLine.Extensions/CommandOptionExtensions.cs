namespace System.CommandLine.Extensions
{
    using Threading.Tasks;

    public static class CommandOptionExtensions
    {
        public static CommandOption Option<T>(this CommandOption commandOption, string template, string description = null)
        {
            if (commandOption == null) throw new ArgumentNullException(nameof(commandOption));


            return commandOption.command.Option<T>(template, description);
        }

        public static void OnExecute(this CommandOption commandOption, Func<Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1>(this CommandOption commandOption, Func<T1, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2>(this CommandOption commandOption, Func<T1, T2, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3>(this CommandOption commandOption, Func<T1, T2, T3, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4>(this CommandOption commandOption, Func<T1, T2, T3, T4, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4, T5>(this CommandOption commandOption, Func<T1, T2, T3, T4, T5, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4, T5, T6>(this CommandOption commandOption, Func<T1, T2, T3, T4, T5, T6, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4, T5, T6, T7>(this CommandOption commandOption, Func<T1, T2, T3, T4, T5, T6, T7, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4, T5, T6, T7, T8>(this CommandOption commandOption, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this CommandOption commandOption, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<int>> execute) => commandOption.command.OnExecute(execute);
        public static void OnExecute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this CommandOption commandOption, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<int>> execute) => commandOption.command.OnExecute(execute);
    }
}