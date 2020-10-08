using System;

namespace CSharpClassReloading
{
    static class InternalClass
    {
        internal static void Call(string reloadMethod)
        {
            System.Diagnostics.Debug.WriteLine("InternalClass " + reloadMethod);
        }
    }
}
