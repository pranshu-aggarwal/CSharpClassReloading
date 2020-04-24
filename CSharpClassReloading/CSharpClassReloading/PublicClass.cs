using System;
namespace CSharpClassReloading
{
    public static class PublicClass
    {
        public static void Call(string reloadMethod)
        {
            Console.WriteLine("PublicClass " + reloadMethod);
        }
    }
}
