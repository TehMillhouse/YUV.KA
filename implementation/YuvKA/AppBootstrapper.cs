﻿namespace YuvKA
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Caliburn.Micro;
	using Caliburn.Micro.Logging;
	using YuvKA.ViewModel;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "God class")]
	public class AppBootstrapper : Bootstrapper<MainViewModel>
	{
		CompositionContainer container;

		/// <summary>
		/// By default, we are configured to use MEF
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "God class")]
		protected override void Configure()
		{
			LogManager.GetLog = t => new DebugLogger(t);
			ViewLocator.NameTransformer.AddRule("ViewModel", "View");

			var catalog = new AggregateCatalog(
				AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
			);

			container = new CompositionContainer(catalog);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue(container);
			batch.AddExportedValue(catalog);

			container.Compose(batch);
		}

		// Return exe and all dlls in Plugins folder.
		protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
		{
			var exeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
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
	}
}
