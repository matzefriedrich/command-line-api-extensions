namespace demo
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Extensions;
    using System.Threading.Tasks;

    internal class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Command("greeting", greeting =>
            {
                greeting.Help("Greets the specified person.");
                greeting.Option<string>("--name", "The person´s name.", ArgumentArity.ExactlyOne)
                    .Option<bool>("--polite")
                    .OnExecute(async (string name, bool polite) =>
                    {
                        if (polite) Console.WriteLine($"Good day {name}");
                        else Console.WriteLine($"Hello {name}");
                        return await Task.FromResult(0);
                    });
            });

            return app.Execute(args);
        }
    }
}