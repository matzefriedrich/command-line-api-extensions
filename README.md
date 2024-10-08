![.NET 8](https://github.com/matzefriedrich/command-line-api-extensions/actions/workflows/dotnet.yml/badge.svg)
![GitHub License](https://img.shields.io/github/license/matzefriedrich/command-line-api-extensions)
![GitHub Release](https://img.shields.io/github/v/release/matzefriedrich/command-line-api-extensions?include_prereleases&display_name=tag)

# System.CommandLine.Extensions

The new `System.CommandLine` API offers for sure more advanced command configuration and execution than the retired `Microsoft.Extensions.CommandLineUtils` API. But the broader set of functionality comes with the burden of more complex boilerplate code required to let the cow fly. The `System.CommandLine.Extensions` API adds a thin application layer to `System.CommandLine` which is similar to the retired API, cuts down functionality and thus brings back the simplicity.

## Greeting Example

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
````
