namespace YuvKA
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Input;
	using Caliburn.Micro;
	using YuvKA.ViewModel;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "God class")]
	public class AppBootstrapper : Bootstrapper<MainViewModel>
	{
		static readonly string exeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		CompositionContainer container;

		/// <summary>
		/// By default, we are configured to use MEF
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "God class")]
		protected override void Configure()
		{
			LogManager.GetLog = _ => new Logger();
			ViewLocator.NameTransformer.AddRule(@"ViewModel(?=$|\.)", "View");
			ViewLocator.NameTransformer.AddRule(@"ViewModels(?=$|\.)", "Views");

			var catalog = new AggregateCatalog(
				AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
			);

			container = new CompositionContainer(catalog);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue<IGetPosition>(new WpfGetPosition());
			batch.AddExportedValue(container);
			batch.AddExportedValue(catalog);

			container.Compose(batch);
		}

		// Return exe and all dlls in Plugins folder.
		protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
		{
			var pluginDir = Path.Combine(exeDir, "Plugins");
			return base.SelectAssemblies().Concat(Directory.EnumerateFiles(pluginDir, "*.dll").Select(Assembly.LoadFile));
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			var exports = container.GetExportedValues<object>(contract);

			if (exports.Count() > 0)
				return exports.First();

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
		}

		protected override void BuildUp(object instance)
		{
			container.SatisfyImportsOnce(instance);
		}

		class Logger : ILog
		{
			static readonly string logFile = Path.Combine(exeDir, "Caliburn.log");
			public Logger()
			{
				File.Delete(logFile);
			}

			public void Error(Exception exception)
			{
				Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
				throw exception;
			}

			public void Info(string format, params object[] args)
			{
				File.AppendAllText(logFile, "\r\nINFO: " + CreateLogMessage(format, args));
			}

			public void Warn(string format, params object[] args)
			{
				Debug.WriteLine(CreateLogMessage(format, args), "WARN");
			}

			string CreateLogMessage(string format, params object[] args)
			{
				return string.Format("[{0}] {1}", DateTime.Now.ToString("o"), string.Format(format, args));
			}
		}

		class WpfGetPosition : IGetPosition
		{
			public Point GetMousePosition(MouseEventArgs e, IViewAware relativeTo)
			{
				return e.GetPosition((IInputElement)relativeTo.GetView());
			}

			public Point GetDropPosition(DragEventArgs e, IViewAware relativeTo)
			{
				return e.GetPosition((IInputElement)relativeTo.GetView());
			}

			public Point? GetElementPosition(IViewAware element, IViewAware relativeTo)
			{
				if (element.GetView() == null)
					return null;

				return ((UIElement)element.GetView()).TranslatePoint(new Point(), (UIElement)relativeTo.GetView());
			}

			public Size GetElementSize(IViewAware element)
			{
				return ((UIElement)element.GetView()).RenderSize;
			}
		}
	}
}
