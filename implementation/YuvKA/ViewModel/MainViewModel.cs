using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Win32;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class MainViewModel
	{
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

		public ReplayStateViewModel ReplayStateViewModel { get; private set; }
		public PipelineViewModel PipelineViewModel { get; private set; }
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
			OpenWindows.Clear();
			undoStack.Clear();
			redoStack.Clear();
		}

		public void Undo()
		{
			redoStack.Clear();
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
