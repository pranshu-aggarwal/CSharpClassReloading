using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Reloading.Core
{
    public class CSharpScriptLoader
    {
        public static ICompiler Instance { get; set; }
        public static async Task<Type> Load(string code, string className)
        {
            return await Instance.Load(code, className);
        }
    }

    public interface ICompiler
    {
        Task<Type> Load(string code, string className);
    }

    public class RoslynCompiler 
    {
        public static async Task<Type> Load(string code, string className)
        {
            var script = CSharpScript.Create(code, ScriptOptions.Default.WithReferences(Assembly.GetExecutingAssembly()));
            script.Compile();
            var state = await script.RunAsync();
            state = await state.ContinueWithAsync($"return typeof({className});");

            return (Type)state.ReturnValue;
        }
    }
}
