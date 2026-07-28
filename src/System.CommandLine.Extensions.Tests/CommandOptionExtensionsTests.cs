namespace System.CommandLine.Extensions.Tests;

public class CommandOptionExtensionsTests
{
    [Fact]
    public void CommandOptionExtensions_Option_chains_multiple_options()
    {
        // Arrange
        var app = new CommandLineApplication();
        var capturedValue1 = string.Empty;
        var capturedValue2 = 0;

        // Act
        app.Option<string>("--opt1")
            .Option<int>("--opt2")
            .OnExecute(async (string v1, int v2) =>
            {
                capturedValue1 = v1;
                capturedValue2 = v2;
                return await Task.FromResult(0);
            });

        app.Execute("--opt1", "hello", "--opt2", "42");

        // Assert
        Assert.Equal("hello", capturedValue1);
        Assert.Equal(42, capturedValue2);
    }

    [Fact]
    public void CommandOptionExtensions_OnExecute_registers_handler_from_option()
    {
        // Arrange
        var app = new CommandLineApplication();
        var invoked = false;

        // Act
        app.Option<string>("--some-option")
            .OnExecute(async () =>
            {
                invoked = true;
                return await Task.FromResult(0);
            });

        app.Execute("--some-option", "value");

        // Assert
        Assert.True(invoked);
    }
}