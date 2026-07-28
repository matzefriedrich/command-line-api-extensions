namespace System.CommandLine.Extensions.Tests;

public class CommandLineApplicationTests
{
    [Fact]
    public void CommandLineApplication_Command_adds_subcommand()
    {
        // Arrange
        var app = new CommandLineApplication();
        var subCommandName = "sub";
        var subCommandDescription = "Sub command description";
        var wasFactoryCalled = false;

        // Act
        app.Command(subCommandName, subCommandDescription, factory => { wasFactoryCalled = true; });

        // Assert
        Assert.True(wasFactoryCalled);
        // Indirectly verify by executing the subcommand and seeing it doesn't fail due to unknown command
        var result = app.Execute(subCommandName);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CommandLineApplication_Option_adds_option()
    {
        // Arrange
        var app = new CommandLineApplication();
        var optionValue = string.Empty;
        app.Option<string>("--name", "Name option")
            .OnExecute(async (string name) =>
            {
                optionValue = name;
                return await Task.FromResult(0);
            });

        // Act
        app.Execute("--name", "Junie");

        // Assert
        Assert.Equal("Junie", optionValue);
    }

    [Fact]
    public void CommandLineApplication_Argument_adds_argument()
    {
        // Arrange
        var app = new CommandLineApplication();
        app.Argument<string>("name", "Name argument");

        // Act
        var result = app.Execute("Junie");

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CommandLineApplication_Help_sets_description()
    {
        // Arrange
        var app = new CommandLineApplication();
        var description = "App description";

        // Act
        app.Help(description);

        // Assert
        // We can't easily check the description without reflection, but we can verify it doesn't crash
        var result = app.Execute("--help");
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CommandLineApplication_ExecuteAsync_invokes_async_handler()
    {
        // Arrange
        var app = new CommandLineApplication();
        var invoked = false;
        app.OnExecute(async () =>
        {
            invoked = true;
            await Task.Delay(10);
            return 42;
        });

        // Act
        var result = await app.ExecuteAsync();

        // Assert
        Assert.True(invoked);
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task CommandLineApplication_OnExecute_binds_multiple_options()
    {
        // Arrange
        var app = new CommandLineApplication();
        var capturedName = string.Empty;
        var capturedAge = 0;
        var capturedPolite = false;

        app.Option<string>("--name")
            .Option<int>("--age")
            .Option<bool>("--polite")
            .OnExecute(async (string name, int age, bool polite) =>
            {
                capturedName = name;
                capturedAge = age;
                capturedPolite = polite;
                return await Task.FromResult(0);
            });

        // Act
        await app.ExecuteAsync("--name", "Junie", "--age", "25", "--polite");

        // Assert
        Assert.Equal("Junie", capturedName);
        Assert.Equal(25, capturedAge);
        Assert.True(capturedPolite);
    }

    [Fact]
    public async Task CommandLineApplication_OnExecute_binds_eight_options()
    {
        // Arrange
        var app = new CommandLineApplication();
        var capturedValues = new object[8];

        app.Option<string>("-1").Option<string>("-2").Option<string>("-3").Option<string>("-4")
            .Option<string>("-5").Option<string>("-6").Option<string>("-7").Option<string>("-8")
            .OnExecute(async (string v1, string v2, string v3, string v4, string v5, string v6, string v7, string v8) =>
            {
                capturedValues[0] = v1;
                capturedValues[1] = v2;
                capturedValues[2] = v3;
                capturedValues[3] = v4;
                capturedValues[4] = v5;
                capturedValues[5] = v6;
                capturedValues[6] = v7;
                capturedValues[7] = v8;
                return await Task.FromResult(0);
            });

        // Act
        await app.ExecuteAsync("-1", "a", "-2", "b", "-3", "c", "-4", "d", "-5", "e", "-6", "f", "-7", "g", "-8", "h");

        // Assert
        Assert.Equal(new[] { "a", "b", "c", "d", "e", "f", "g", "h" }, capturedValues.Cast<string>());
    }

    [Fact]
    public void CommandLineApplication_Execute_returns_exit_code()
    {
        // Arrange
        var app = new CommandLineApplication();
        app.OnExecute(() => Task.FromResult(123));

        // Act
        var result = app.Execute();

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void CommandLineApplicationExtensions_Command_adds_subcommand_without_description()
    {
        // Arrange
        var app = new CommandLineApplication();
        var invoked = false;

        // Act
        app.Command("sub", factory => invoked = true);
        app.Execute("sub");

        // Assert
        Assert.True(invoked);
    }

    [Fact]
    public async Task CommandLineApplication_ConfigureAndExecuteAsync_runs_configured_app()
    {
        // Arrange
        var config = new TestConfiguration();

        // Act
        var result = await CommandLineApplication.ConfigureAndExecuteAsync(config, "test --value 123");

        // Assert
        Assert.Equal(123, config.CapturedValue);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CommandLineApplication_Option_sets_required_for_non_nullable_types()
    {
        // Arrange
        var app = new CommandLineApplication();

        // Act
        // This is tricky to verify without reflection, but we can try to execute without it
        app.Option<int>("--required");
        app.OnExecute(() => Task.FromResult(0));

        var result = app.Execute(); // Should fail because --required is missing and it's non-nullable

        // Assert
        Assert.NotEqual(0, result);
    }

    [Fact]
    public void CommandLineApplication_Option_sets_not_required_for_nullable_types()
    {
        // Arrange
        var app = new CommandLineApplication();

        // Act
        app.Option<int?>("--optional");
        app.OnExecute(() => Task.FromResult(0));

        var result = app.Execute(); // Should succeed because --optional is nullable

        // Assert
        Assert.Equal(0, result);
    }

    private class TestConfiguration : ICommandsConfiguration
    {
        public int CapturedValue { get; private set; }

        public void Configure(CommandLineApplication app)
        {
            app.Command("test", test =>
            {
                test.Option<int>("--value")
                    .OnExecute(async (int value) =>
                    {
                        this.CapturedValue = value;
                        return await Task.FromResult(0);
                    });
            });
        }
    }
}