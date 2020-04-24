using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Test;

namespace CSharpClassReloading
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("1. Public Class");
            Console.WriteLine("2. Internal Class");
            var classType = int.Parse(Console.ReadLine());
            string code = GetCSharpScriptCode(classType, "CSharpClassReloading.dll");

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var type = await CSharpScriptLoader.Load(code, "NewClass");
                var executeMethod = type.GetMethod("Execute");
                executeMethod.Invoke(null, null);
                Console.WriteLine("Total Executed Time: " + sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

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
