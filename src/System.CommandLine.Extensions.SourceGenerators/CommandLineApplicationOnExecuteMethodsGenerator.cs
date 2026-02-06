namespace System.CommandLine.Extensions.SourceGenerators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using Text;

    [Generator]
    public sealed class CommandLineApplicationOnExecuteMethodsGenerator : IIncrementalGenerator
    {
        private const int NumOverloads = 8;

        private const string NamespaceName = "System.CommandLine.Extensions";
        private const string TypeName = "CommandLineApplication";

        private static readonly IEnumerable<string> UsingList = new List<string>
        {
            "Collections.Generic",
            "System.CommandLine.Binding",
            "System.Linq",
            "System.Threading.Tasks"
        };

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static ctx =>
            {
                var sourceText = GenerateSource();
                ctx.AddSource(nameof(CommandLineApplicationOnExecuteMethodsGenerator), sourceText);
            });
        }

        private static SourceText GenerateSource()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"namespace {NamespaceName}");
            builder.AppendBlock(ns =>
            {
                foreach (var importedNamespace in UsingList.OrderBy(s => s)) ns.AppendLine($"using {importedNamespace};");

                ns.AppendLine($"partial class {TypeName}");
                ns.AppendBlock(t =>
                {
                    for (var i = 1; i <= NumOverloads; i++)
                    {
                        var methodArgumentsCount = i;
                        var typeArgumentsString = string.Join(", ", Enumerable.Range(1, methodArgumentsCount).Select(typeParameterIndex => $"T{typeParameterIndex}"));

                        t.AppendLine($"public void OnExecute<{typeArgumentsString}>(Func<{typeArgumentsString}, Task<int>> execute)");
                        t.AppendBlock(methodBody =>
                        {
                            methodBody.AppendLine("IReadOnlyList<Option> options = this.command.Options.ToList();");

                            var parameters = new List<string>();
                            for (var j = 0; j < methodArgumentsCount; j++)
                            {
                                var parameterIndex = j + 1;
                                var variableName = $"descriptor{parameterIndex}";
                                methodBody.AppendLine($"IValueDescriptor<T{parameterIndex}> {variableName} = this.FindValueDescriptorForOption<T{parameterIndex}>({j});");
                                parameters.Add(variableName);
                            }

                            var parametersString = string.Join(", ", parameters);
                            methodBody.AppendLine($"this.command.SetHandler(execute, {parametersString});");
                        });
                    }
                });

                ns.AppendLine("partial class CommandOptionExtensions");
                ns.AppendBlock(t =>
                {
                    for (var i = 1; i <= NumOverloads; i++)
                    {
                        var methodArgumentsCount = i;
                        var typeArgumentsString = string.Join(", ", Enumerable.Range(1, methodArgumentsCount).Select(typeParameterIndex => $"T{typeParameterIndex}"));

                        t.AppendLine($"public static void OnExecute<{typeArgumentsString}>(this CommandOption commandOption, Func<{typeArgumentsString}, Task<int>> execute) => commandOption.Command.OnExecute(execute);");
                    }
                });
            });

            return SourceText.From(builder.ToString(), Encoding.UTF8);
        }
    }

    internal static class StringBuilderExtensions
    {
        public static void AppendBlock(this StringBuilder builder, Action<IndentedStringWriter> blockWriter)
        {
            builder.AppendLine("{");
            var writer = new IndentedStringWriter(builder, 1);
            blockWriter(writer);
            builder.AppendLine("}");
        }
    }

    internal sealed class IndentedStringWriter(StringBuilder stringBuilder, int indentionLevel)
    {
        private readonly string indentionString = new(' ', indentionLevel * 4);

        public void AppendBlock(Action<IndentedStringWriter> blockWriter)
        {
            stringBuilder.Append(this.indentionString);
            stringBuilder.AppendLine("{");

            var writer = new IndentedStringWriter(stringBuilder, indentionLevel + 1);
            blockWriter(writer);

            stringBuilder.Append(this.indentionString);
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine();
        }

        public void AppendLine(string text)
        {
            stringBuilder.Append(this.indentionString);
            stringBuilder.AppendLine(text);
        }
    }
}