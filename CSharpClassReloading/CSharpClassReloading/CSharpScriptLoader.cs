using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpClassReloading
{
    public class CSharpScriptLoader
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
