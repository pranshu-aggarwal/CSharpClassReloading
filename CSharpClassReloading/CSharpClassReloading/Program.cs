using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Reloading.Core;

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
                        string code = CodeGenerator.GetCSharpScriptCode(classType == 1, "CSharpClassReloading.dll");
                        type = await CSharpScriptLoader.Load(code, "NewClass");
                        break;
                    default:
                        code = CodeGenerator.GetCSharpCompilationCode(classType == 1);
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

        

        
    }
}