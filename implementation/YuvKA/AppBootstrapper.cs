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
	using System.Reactive;
	using System.Reactive.Linq;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Input;
	using Caliburn.Micro;
	using YuvKA.ViewModel;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "God class")]
	public class AppBootstrapper : Bootstrapper<MainViewModel>
	{
		static AppBootstrapper designInstance;

		CompositionContainer container;

		public static MainViewModel DesignRoot
		{
			get
			{
				if (designInstance == null) {
					designInstance = new AppBootstrapper();
					var node1 = new DesignDummyNode { X = 10, Y = 10 };
					var node2 = new DesignDummyNode { X = 200, Y = 10 };
					node2.Inputs[1].Source = node1.Outputs[0];
					DesignRoot.Model.Graph.AddNodeWithIndex(node1);
					DesignRoot.Model.Graph.AddNodeWithIndex(node2);
					DesignRoot.Model = DesignRoot.Model; // Force PipelineVM re-creation
				}
				return IoC.Get<MainViewModel>();
			}
		}

		static string ExeDir { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }

		/// <summary>
		/// By default, we are configured to use MEF
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "God class")]
		protected override void Configure()
		{
			ViewLocator.NameTransformer.AddRule(@"ViewModel(?=$|\.)", "View");
			ViewLocator.NameTransformer.AddRule(@"ViewModels(?=$|\.)", "Views");

			var catalog = new AggregateCatalog(
				AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
			);

			container = new CompositionContainer(catalog);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IWindowManagerEx>(new WindowManagerEx());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue<IGetPosition>(new WpfGetPosition());
			batch.AddExportedValue(container);
			batch.AddExportedValue(catalog);

			if (Execute.InDesignMode)
				batch.AddExportedValue(new DesignDummyNode());

			container.Compose(batch);
		}

		protected override void StartRuntime()
		{
			LogManager.GetLog = _ => new Logger();
			base.StartRuntime();
		}

		// Return exe and all dlls in Plugins folder.
		protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
		{
			if (Execute.InDesignMode)
				return base.SelectAssemblies();

			var pluginDir = Path.Combine(ExeDir, "Plugins");
			return base.SelectAssemblies().Concat(Directory.EnumerateFiles(pluginDir, "*.dll").Select(Assembly.LoadFile));
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			return GetInstance(serviceType, key, container);
		}

		public static object GetInstance(Type serviceType, string key, CompositionContainer container)
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
			static readonly string logFile = Path.Combine(ExeDir, "Caliburn.log");
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
			public IObservable<Unit> ViewLoaded(IViewAware element)
			{
				IObservable<Unit> getView = element.GetView() != null ? Observable.Return(Unit.Default) :
					Observable.FromEventPattern(element, "ViewAttached").Select(_ => Unit.Default).Take(1);
				return
					from gotView in getView
					from loaded in Observable.FromEventPattern(element.GetView(), "Loaded").Take(1)
					select Unit.Default;
			}

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

			/// <summary>
			/// Work around weird WPF bug...
			/// ...by rounding element sizes, lolwtfbbq
			/// </summary>
			public void FixUpSize(IViewAware element)
			{
				var view = (FrameworkElement)element.GetView();
				view.Width = (int)view.ActualWidth;
				view.Height = (int)view.ActualHeight;
			}
		}

		class WindowManagerEx : WindowManager, IWindowManagerEx
		{
			public void SetOwner(IViewAware owningWindow, IViewAware ownedWindow)
			{
				((Window)ownedWindow.GetView()).Owner = (Window)owningWindow.GetView();
			}
		}

		class DesignDummyNode : YuvKA.Pipeline.Node
		{
			public DesignDummyNode()
				: base(2, 1)
			{
				Name = "Dummy";
			}

			public override VideoModel.Frame[] Process(VideoModel.Frame[] inputs, int tick)
			{
				throw new NotImplementedException();
			}
		}
	}
}
