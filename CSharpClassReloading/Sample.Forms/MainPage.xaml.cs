using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Reloading.Core;
using Xamarin.Forms;

namespace Sample.Forms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();


            //CSharpCompilation.IsChecked = true;
            //Public.IsChecked = true;
        }

        async void Compile_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                Type type;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (CSharpScript.IsChecked)
                {

                    string code = CodeGenerator.GetCSharpScriptCode(Public.IsChecked, "Sample.Forms");
                    type = await CSharpScriptLoader.Load(code, "NewClass");
                }
                else
                {
                    string code = CodeGenerator.GetCSharpCompilationCode(Public.IsChecked);
                    var assembly = Assembly.GetExecutingAssembly();
                    type = await CSharpCompilationLoader.Load(code, "CSharpClassReloading.NewClass");
                }

                var executeMethod = type.GetMethod("Execute");
                executeMethod.Invoke(null, null);
                Result.Text = ("Total Executed Time: " + sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
            }
        }

        void CSharpScript_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            CSharpCompilation.IsChecked = !e.Value;
        }

        void CSharpCompilation_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            CSharpScript.IsChecked = !e.Value;
        }

        void Public_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            Internal.IsChecked = !e.Value;
        }

        void Internal_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            Public.IsChecked = !e.Value;
        }
    }
}
