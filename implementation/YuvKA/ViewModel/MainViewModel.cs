using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class MainViewModel
	{
		Stack<PipelineState> undoStack = new Stack<PipelineState>();
		Stack<PipelineState> redoStack = new Stack<PipelineState>();

		public PipelineState Model { get; private set; }
		public bool CanUndo { get { return undoStack.Any(); } }
		public bool CanRedo { get { return redoStack.Any(); } }

		public ReplayStateViewModel ReplayStateViewModel { get; private set; }
		public PipelineViewModel PipelineViewModel { get; private set; }
		public ToolboxViewModel ToolboxViewModel { get; private set; }
		public IList<OutputWindowViewModel> OpenWindows { get; private set; }

		public void Save()
		{
			throw new System.NotImplementedException();
		}

		public void Open()
		{
			throw new System.NotImplementedException();
		}

		public void Clear()
		{
			throw new System.NotImplementedException();
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
