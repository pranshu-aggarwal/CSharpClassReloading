using System;

namespace Reloading.Core
{
    public static class CodeGenerator
    {
        public static string GetCSharpScriptCode(bool isPublicClass, string assemblyName)
        {
            string methodCall = "";
            if (isPublicClass)
            {
                methodCall = "CSharpClassReloading.PublicClass.Call(\"CSharpScriptLoader\");";
            }
            else
            {
                methodCall = $"var assembly = System.Reflection.Assembly.GetCallingAssembly();";
                //methodCall = $"var assembly = System.Reflection.Assembly.LoadFrom(\"{assemblyName}\");";
                methodCall += "var type = assembly.GetType(\"CSharpClassReloading.InternalClass\");";
                methodCall += "var mi = type.GetMethod(\"Call\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);";
                methodCall += "mi.Invoke(null, new[] {\"CSharpScriptLoader\"});";
            }
            return @"public static class NewClass
{
	public static void Execute()
	{"
+ methodCall +
    @"}
    }
";
        }

        public static string GetCSharpCompilationCode(bool isPublicClass)
        {
            if (isPublicClass)
                return GetCSharpCompilationCodePublic();

            return GetCSharpCompilationCodeInternal();
        }

        private static string GetCSharpCompilationCodePublic()
        {
            return @"namespace CSharpClassReloading
{
    public static class NewClass
    {
	    public static void Execute()
	    {"
    + "CSharpClassReloading.PublicClass.Call(\"CSharpScriptCompilation\");" +
        @"}
        }
}
";
        }

        public static string GetCSharpCompilationCodeInternal()
        {
            return "[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo(\"CSharpClassReloading\")]" + @"
namespace CSharpClassReloading
{
    public static class NewClass
    {
	    public static void Execute()
	    {"
    + "CSharpClassReloading.InternalClass.Call(\"CSharpScriptCompilation\");" +
        @"}
        }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
    }
}
";
        }
    }
}
