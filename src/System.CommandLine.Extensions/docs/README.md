A slim command configuration and execution layer similar to `Microsoft.Extensions.CommandlineUtils`, but built on top of the new `System.Commandline` API.

## Usage

````csharp
private static int Main(string[] args)
{
    var app = new CommandlineApplication();

    app.Command("greeting", "Greets the specified person.", greeting =>
    {
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
````

### Usage

````bash
$ demo greeting --name Jon --polite
Good day Jon
````
