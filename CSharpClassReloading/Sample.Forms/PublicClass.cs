using System;
namespace CSharpClassReloading
{
    public static class PublicClass
    {
        public static void Call(string reloadMethod)
        {
            System.Diagnostics.Debug.WriteLine("PublicClass " + reloadMethod);
        }
    }
}
