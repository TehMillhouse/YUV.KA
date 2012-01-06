using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;
using Microsoft.Win32;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	[Export]
	public class MainViewModel : PropertyChangedBase
	{
		Stack<byte[]> undoStack = new Stack<byte[]>();
		Stack<byte[]> redoStack = new Stack<byte[]>();
		PipelineState model;
		PipelineViewModel pipelineViewModel;

		[ImportingConstructor]
		public MainViewModel(CompositionContainer container)
		{
			Container = container;
			OpenWindows = new List<OutputWindowViewModel>();
			Clear();
		}

		public PipelineState Model
		{
			get { return model; }
			set
			{
				model = value;
				Container.SatisfyImportsOnce(model);
				if (ReplayStateViewModel != null)
					ReplayStateViewModel.IsPlaying = false;
				PipelineViewModel = new PipelineViewModel(this);
			}
		}

		public bool CanUndo { get { return undoStack.Any(); } }
		public bool CanRedo { get { return redoStack.Any(); } }

		[Import]
		public ReplayStateViewModel ReplayStateViewModel { get; private set; }

		public PipelineViewModel PipelineViewModel
		{
			get { return pipelineViewModel; }
			set
			{
				pipelineViewModel = value;
				NotifyOfPropertyChange(() => PipelineViewModel);
			}
		}

		[Import]
		public ToolboxViewModel ToolboxViewModel { get; private set; }

		public IList<OutputWindowViewModel> OpenWindows { get; private set; }

		CompositionContainer Container { get; set; }

		public void Save()
		{
			var dialog = new SaveFileDialog();
			if (dialog.ShowDialog() == true) {
				var serializer = new NetDataContractSerializer();
				File.WriteAllBytes(dialog.FileName, Serialize(Model));
			}
		}

		public void Open()
		{
			var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == true)
				Model = Deserialize(File.ReadAllBytes(dialog.FileName));
		}

		public void Clear()
		{
			Model = new PipelineState();

			OpenWindows.Clear();
			undoStack.Clear();
			redoStack.Clear();
		}

		public void Undo()
		{
			redoStack.Push(Serialize(Model));
			Model = Deserialize(undoStack.Pop());
		}

		public void Redo()
		{
			undoStack.Push(Serialize(Model));
			Model = Deserialize(redoStack.Pop());
		}

		public void SaveSnapshot()
		{
			redoStack.Clear();
			undoStack.Push(Serialize(Model));
		}

		public void OpenWindow(OutputWindowViewModel window)
		{
			OpenWindows.Add(window);
			Model.RenderTick(new[] { window.NodeModel });
		}

		byte[] Serialize(PipelineState state)
		{
			using (var stream = new MemoryStream()) {
				new NetDataContractSerializer().Serialize(stream, state);
				return stream.ToArray();
			}
		}

		PipelineState Deserialize(byte[] data)
		{
			OpenWindows.Clear(); // TODO: for now...
			using (var stream = new MemoryStream(data))
				return (PipelineState)new NetDataContractSerializer().Deserialize(stream);
		}
	}
}
