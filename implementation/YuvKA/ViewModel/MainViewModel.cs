using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.Pipeline;
using System.ComponentModel.Composition;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.IO;

namespace YuvKA.ViewModel
{
	public class MainViewModel
	{
		Stack<PipelineState> undoStack = new Stack<PipelineState>();
		Stack<PipelineState> redoStack = new Stack<PipelineState>();

		public PipelineState Model { get { throw new NotImplementedException(); } }
		public bool CanUndo { get { return undoStack.Any(); } }
		public bool CanRedo { get { return redoStack.Any(); } }

		public ReplayStateViewModel ReplayStateViewModel { get; private set; }
		public PipelineViewModel PipelineViewModel { get { throw new NotImplementedException(); } }
		public ToolboxViewModel ToolboxViewModel { get; private set; }
		public IList<OutputWindowViewModel> OpenWindows { get { throw new NotImplementedException(); } }

		public void Save()
		{
			var dialog = new SaveFileDialog();
			if (dialog.ShowDialog() == true) {
				var serializer = new NetDataContractSerializer();
				using (Stream stream = dialog.OpenFile())
					serializer.WriteObject(stream, Model);
			}
		}

		public void Open()
		{
			var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == true) {
				var serializer = new NetDataContractSerializer();
				using (Stream stream = dialog.OpenFile())
					Model = (PipelineState)serializer.ReadObject(stream);
			}
		}

		public void Clear()
		{
			Model = new PipelineState();
		}

		public void Undo()
		{
			redoStack.Clear();
			undoStack.Push(Model);
		}

		public void Redo()
		{
			undoStack.Push(Model);
			Model = redoStack.Pop();
		}

		public void SaveSnapshot()
		{
			throw new System.NotImplementedException();
		}
	}
}
