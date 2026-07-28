namespace System.CommandLine.Extensions;

using Collections.Generic;
using Diagnostics.CodeAnalysis;
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

    private CommandLineApplication(Command command)
    {
        this.command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public CommandLineApplication Command(string name, string? description = null, Action<CommandLineApplication>? factory = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        var subCommand = new Command(name, description);
        this.command.Add(subCommand);
        var wrapper = new CommandLineApplication(subCommand);
        factory?.Invoke(wrapper);
        return this;
    }

    public CommandOption Option<T>(string template, string? description = null, ArgumentArity argumentArity = default)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(template));
        }

        var aliases = template.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
        var argumentType = typeof(T);
        var name = aliases.First();
        var option = new Option<T>(name)
        {
            Arity = argumentArity,
            Description = description,
            Required = Nullable.GetUnderlyingType(argumentType) == null
        };

        this.command.Add(option);
        return new CommandOption(this, option);
    }

    public void OnExecute(Func<Task<int>> execute)
    {
        this.command.SetAction(async (result, token) =>
        {
            return await execute();
        });
    }

    private Option<T> FindValueDescriptorForOption<T>(int index)
    {
        IReadOnlyList<Option> options = this.command.Options.ToList();
        if (index >= 0 && index < options.Count)
        {
            return (Option<T>)options[index];
        }

        return new Option<T>($"missing{index}");
    }

    public CommandLineApplication Argument<T>(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        var argument = new Argument<T>(name) { Description = description };
        this.command.Add(argument);
        return this;
    }

    public CommandLineApplication Help(string description)
    {
        this.command.Description = description;
        return this;
    }

    public int Execute(params string[] args)
    {
        var parserConfiguration = new ParserConfiguration { EnablePosixBundling = true };
        var parseResult = this.command.Parse(args, parserConfiguration);
        var invocationConfiguration = new InvocationConfiguration { EnableDefaultExceptionHandler = false };
        return parseResult.Invoke(invocationConfiguration);
    }

    public async Task<int> ExecuteAsync(params string[] args)
    {
        var parserConfiguration = new ParserConfiguration { EnablePosixBundling = true };
        var parseResult = this.command.Parse(args, parserConfiguration);
        var invocationConfiguration = new InvocationConfiguration { EnableDefaultExceptionHandler = false };
        return await parseResult.InvokeAsync(invocationConfiguration);
    }

    public static async Task<int> ConfigureAndExecuteAsync(ICommandsConfiguration configuration, string command)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var app = new CommandLineApplication();
        configuration.Configure(app);

        var parseResult = app.command.Parse(command);
        var invocationConfiguration = new InvocationConfiguration { EnableDefaultExceptionHandler = false };
        return await parseResult.InvokeAsync(invocationConfiguration);
    }
}