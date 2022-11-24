namespace System.CommandLine.Extensions.SourceGenerators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using Text;

    [Generator]
    public sealed class CommandLineApplicationOnExecuteMethodsGenerator : ISourceGenerator
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


        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"namespace {NamespaceName}");
            builder.AppendBlock(ns =>
            {
                foreach (string importedNamespace in UsingList.OrderBy(s => s))
                {
                    ns.AppendLine($"using {importedNamespace};");
                }

                ns.AppendLine($"partial class {TypeName}");
                ns.AppendBlock(t =>
                {
                    for (var i = 1; i <= NumOverloads; i++)
                    {
                        int methodArgumentsCount = i;
                        string typeArgumentsString = string.Join(", ", Enumerable.Range(1, methodArgumentsCount).Select(typeParameterIndex => $"T{typeParameterIndex}"));

                        t.AppendLine($"public void OnExecute<{typeArgumentsString}>(Func<{typeArgumentsString}, Task<int>> execute)");
                        t.AppendBlock(methodBody =>
                        {
                            methodBody.AppendLine("IReadOnlyList<Option> options = this.command.Options.ToList();");

                            var parameters = new List<string>();
                            for (var j = 0; j < methodArgumentsCount; j++)
                            {
                                int parameterIndex = j + 1;
                                var variableName = $"descriptor{parameterIndex}";
                                methodBody.AppendLine($"IValueDescriptor<T{parameterIndex}> {variableName} = this.FindValueDescriptorForOption<T{parameterIndex}>({j});");
                                parameters.Add(variableName);
                            }

                            string parametersString = string.Join(", ", parameters);
                            methodBody.AppendLine($"this.command.SetHandler(execute, {parametersString});");
                        });
                    }
                });
                
                ns.AppendLine("partial class CommandOptionExtensions");
                ns.AppendBlock(t =>
                {
                    for (var i = 1; i <= NumOverloads; i++)
                    {
                        int methodArgumentsCount = i;
                        string typeArgumentsString = string.Join(", ", Enumerable.Range(1, methodArgumentsCount).Select(typeParameterIndex => $"T{typeParameterIndex}"));

                        t.AppendLine($"public static void OnExecute<{typeArgumentsString}>(this CommandOption commandOption, Func<{typeArgumentsString}, Task<int>> execute) => commandOption.Command.OnExecute(execute);");
                    }
                });
            });

            SourceText sourceText = SourceText.From(builder.ToString(), Encoding.UTF8);
            context.AddSource(nameof(CommandLineApplicationOnExecuteMethodsGenerator), sourceText);
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

    internal sealed class IndentedStringWriter
    {
        private readonly int indentionLevel;
        private readonly string indentionString;
        private readonly StringBuilder stringBuilder;

        public IndentedStringWriter(StringBuilder stringBuilder, int indentionLevel)
        {
            this.stringBuilder = stringBuilder;
            this.indentionLevel = indentionLevel;
            this.indentionString = new string(' ', indentionLevel * 4);
        }

        public void AppendBlock(Action<IndentedStringWriter> blockWriter)
        {
            this.stringBuilder.Append(this.indentionString);
            this.stringBuilder.AppendLine("{");

            var writer = new IndentedStringWriter(this.stringBuilder, this.indentionLevel + 1);
            blockWriter(writer);

            this.stringBuilder.Append(this.indentionString);
            this.stringBuilder.AppendLine("}");
            this.stringBuilder.AppendLine();
        }

        public void AppendLine(string text)
        {
            this.stringBuilder.Append(this.indentionString);
            this.stringBuilder.AppendLine(text);
        }
    }
}