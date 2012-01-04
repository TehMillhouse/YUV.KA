using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Win32;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	[Export]
	public class MainViewModel
	{
		[Import]
		CompositionContainer container;
		Stack<byte[]> undoStack = new Stack<byte[]>();
		Stack<byte[]> redoStack = new Stack<byte[]>();

		public MainViewModel()
		{
			OpenWindows = new List<OutputWindowViewModel>();
			Model = new PipelineState();
		}

		public PipelineState Model { get; private set; }
		public bool CanUndo { get { return undoStack.Any(); } }
		public bool CanRedo { get { return redoStack.Any(); } }

		[Import]
		public ReplayStateViewModel ReplayStateViewModel { get; private set; }
		public PipelineViewModel PipelineViewModel { get; private set; }
		[Import]
		public ToolboxViewModel ToolboxViewModel { get; private set; }
		public IList<OutputWindowViewModel> OpenWindows { get; private set; }

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
			container.SatisfyImportsOnce(Model);

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
			using (var stream = new MemoryStream(data)) {
				var state = (PipelineState)new NetDataContractSerializer().Deserialize(stream);
				container.SatisfyImportsOnce(state);
				return state;
			}
		}
	}
}
