using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Foundation;
using Mono.CSharp;
using Reloading.Core;
using UIKit;

namespace Sample.Forms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            CSharpScriptLoader.Instance = new MonoCompiler();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }

    public class Printer : ReportPrinter
    {
        public override void Print(AbstractMessage msg, bool showFullPath)
        {
            if (!msg.IsWarning)
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

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
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
            if (name.Contains("Sample"))
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
