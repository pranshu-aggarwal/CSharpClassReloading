using System;

namespace CSharpClassReloading
{
    static class InternalClass
    {
        internal static void Call(string reloadMethod)
        {
            Console.WriteLine("InternalClass " + reloadMethod);
        }
    }
}
