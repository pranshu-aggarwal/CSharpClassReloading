using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mono.CSharp;
using Reloading.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace Sample.Forms.Droid
{
    [Activity(Label = "Sample.Forms", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            CSharpScriptLoader.Instance = new MonoCompiler();

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Compile()
        {
            Evaluator evaluator = new Evaluator(new CompilerContext(new CompilerSettings(), new Printer()));
            var result = evaluator.Evaluate("1+2");
        }
    }

    public class Printer : ReportPrinter
    {
        public override void Print(AbstractMessage msg, bool showFullPath)
        {
            if(!msg.IsWarning)
            {

            }
            base.Print(msg, showFullPath);
        }
    }

    public class MonoCompiler : ICompiler
    {
        private List<Assembly> references;
        Evaluator evaluator;
        public MonoCompiler()
        {
            evaluator = new Evaluator(new CompilerContext(new CompilerSettings(), new Printer()));

            references = new List<Assembly>();

            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadAssembly(assembly);
            }

            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LoadAssembly(args.LoadedAssembly);
        }

        void LoadAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            if (name == "mscorlib" || name == "System" || name == "System.Core" || name.StartsWith("eval-"))
                return;
            if(name.Contains("Sample"))
            {

            }
            references.Add(assembly);
            evaluator?.ReferenceAssembly(assembly);
        }

        public Task<Type> Load(string code, string className)
        {
            evaluator.Evaluate(code, out object result, out bool result_set);
            var obj = evaluator.Evaluate($"typeof({className});");
            return Task.FromResult((Type)obj);
        }
    }
}
