using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Reloading.Core
{
    public class CSharpCompilationLoader
    {
        public static async Task<Type> Load(string code, string className)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assembly1 = Assembly.GetExecutingAssembly();
            var expressionAssembly = typeof(System.Linq.Expressions.Expression).Assembly;
            var runtimeAssembly = Assembly.Load("System.Runtime");


            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).
                WithMetadataImportOptions(MetadataImportOptions.All);

            var topLevelBinderFlagsProperty = typeof(CSharpCompilationOptions).GetProperty("TopLevelBinderFlags", BindingFlags.Instance | BindingFlags.NonPublic);
            topLevelBinderFlagsProperty.SetValue(compilationOptions, (uint)1 << 22);

            var compilation = CSharpCompilation.Create("DynamicCrazyProgram", options: compilationOptions)
                .AddReferences(MetadataReference.CreateFromFile(
                            assembly1.Location))
                            .AddReferences(MetadataReference.CreateFromFile(
                            typeof(object).Assembly.Location),
                            MetadataReference.CreateFromFile(expressionAssembly.Location),
                            MetadataReference.CreateFromFile(runtimeAssembly.Location))
                            .AddSyntaxTrees(syntaxTree);

            using (var ms = new MemoryStream())
            {
                var cr = compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());
                return assembly.GetType(className);
            }
        }
    }
}
