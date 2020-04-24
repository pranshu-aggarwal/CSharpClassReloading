using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharpClassReloading
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("1. CSharpScript");
            Console.WriteLine("2. CSharpCompilation");
            var loadType = int.Parse(Console.ReadLine());
            Console.WriteLine("1. Public Class");
            Console.WriteLine("2. Internal Class");
            var classType = int.Parse(Console.ReadLine());
            Type type;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                switch (loadType)
                {
                    case (1):
                        string code = GetCSharpScriptCode(classType, "CSharpClassReloading.dll");
                        type = await CSharpScriptLoader.Load(code, "NewClass");
                        break;
                    default:
                        code = GetCSharpCompilationCode(classType);
                        type = await CSharpCompilationLoader.Load(code, "CSharpClassReloading.NewClass");
                        break;
                }
                var executeMethod = type.GetMethod("Execute");
                executeMethod.Invoke(null, null);
                Console.WriteLine("Total Executed Time: " + sw.ElapsedMilliseconds);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string GetCSharpCompilationCode(int classType)
        {
            if (classType == 1)
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
    + "CSharpClassReloading.PublicClass.Call(\"CSharpScriptLoader\");" +
        @"}
        }
}
";
        }

        private static string GetCSharpCompilationCodeInternal()
        {
            return "[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo(\"CSharpClassReloading\")]" + @"
namespace CSharpClassReloading
{
    public static class NewClass
    {
	    public static void Execute()
	    {"
    + "CSharpClassReloading.InternalClass.Call(\"CSharpScriptLoader\");" +
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

        private static string GetCSharpScriptCode(int classType, string assemblyName)
        {
            string methodCall = "";
            if(classType == 1)
            {
                methodCall = "CSharpClassReloading.PublicClass.Call(\"CSharpScriptLoader\");";
            }
            else
            {
                methodCall = $"var assembly = System.Reflection.Assembly.LoadFrom(\"{assemblyName}\");";
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
    }
}