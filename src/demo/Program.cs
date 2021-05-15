namespace demo
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Extensions;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Command("greeting", greeting =>
            {
                greeting.Help("Greets the specified person.");
                greeting.Option<string>("--name", "The person´s name.", ArgumentArity.ExactlyOne)
                    .Option<bool>("--polite")
                    .OnExecute(async (string name, bool polite, int missing0) =>
                    {
                        Console.WriteLine(polite ? $"Good day {name}" : $"Hello {name}");
                        return await Task.FromResult(0);
                    });
            });

            return await app.ExecuteAsync(args);
        }
    }
}