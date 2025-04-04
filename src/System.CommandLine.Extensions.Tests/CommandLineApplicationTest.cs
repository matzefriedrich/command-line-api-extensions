namespace System.CommandLine.Extensions.Tests
{
    using Xunit;

    public class CommandLineApplicationTest
    {
        [Fact]
        public void Command_with_optional_argument_must_not_fail_if_omitted()
        {   
            // Arrange
            var sut = new CommandLineApplication();

            string? actual = null;
            sut.Command("test", "Test command", cmd =>
            {
                cmd
                    .Option<string?>("--optional", "Optional argument", ArgumentArity.ExactlyOne)
                    .OnExecute<string?>(s =>
                    {
                        actual = s;
                        return Task.FromResult(0);
                    });
            });

            // Act
            sut.Execute("test");

            // Assert
            Assert.Null(actual);
        }
        
        [Fact]
        public void Command_with_optional_argument_must_not_fail()
        {   
            // Arrange
            var sut = new CommandLineApplication();

            string? actual = null;
            sut.Command("test", "Test command", cmd =>
            {
                cmd
                    .Option<string?>("--optional", "Optional argument", ArgumentArity.ExactlyOne)
                    .OnExecute<string?>(s =>
                    {
                        actual = s;
                        return Task.FromResult(0);
                    });
            });

            // Act
            sut.Execute("test", "--optional", "value");

            // Assert
            Assert.Equal("value", actual);
        }
    }
}