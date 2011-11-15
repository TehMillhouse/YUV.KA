using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

//
// SketchFlow benötigt Angaben darüber, welche Steuerelementassembly in seinen Fenstern enthalten ist. Dies wird automatisch
// bei der Projekterstellung festgelegt. Wenn aber der Name der Steuerelementassembly manuell geändert wird, dann muss der Name hier
// auch manuell aktualisiert werden.
//
[assembly: Microsoft.Expression.Prototyping.Services.SketchFlowLibraries("WpfPrototype.Screens")]

namespace WpfPrototype
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.App_Startup;
		}

		private void App_Startup(object sender, StartupEventArgs e)
		{
			this.StartupUri = new Uri(@"pack://application:,,,/Microsoft.Expression.Prototyping.Runtime;Component/WPF/Workspace/PlayerWindow.xaml");
		}
	}
}