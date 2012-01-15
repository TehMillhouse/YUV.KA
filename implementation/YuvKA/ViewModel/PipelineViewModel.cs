using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;
using System.Windows.Input;

namespace YuvKA.ViewModel
{
	public class PipelineViewModel : ViewAware
	{
		NodeViewModel draggedNode;
		Vector dragMouseOffset;

		public PipelineViewModel(MainViewModel parent)
		{
			Parent = parent;
			Nodes = new ObservableCollection<NodeViewModel>(Parent.Model.Graph.Nodes.Select(n => new NodeViewModel(n, Parent)));
		}

		public MainViewModel Parent { get; private set; }
		public IList<NodeViewModel> Nodes { get; private set; }
		public IEnumerable<EdgeViewModel> Edges { get; private set; }

		public void Drop(DragEventArgs e)
		{
			var type = (NodeType)e.Data.GetData(typeof(NodeType));
			var node = (Node)Activator.CreateInstance(type.Type);
			var nodeModel = new NodeViewModel(node, Parent);
			nodeModel.Position = IoC.Get<IGetPosition>().GetDropPosition(e, this);

			Parent.Model.Graph.Nodes.Add(node);
			Nodes.Add(nodeModel);
		}

		public void NodeMouseDown(NodeViewModel node, MouseEventArgs e)
		{
			draggedNode = node;
			dragMouseOffset = IoC.Get<IGetPosition>().GetMousePosition(e, this) - draggedNode.Position;
		}

		public void MouseMove(MouseEventArgs e)
		{
			if (draggedNode == null || e.LeftButton != MouseButtonState.Pressed) {
				draggedNode = null;
				return;
			}
			draggedNode.Position = IoC.Get<IGetPosition>().GetMousePosition(e, this) - dragMouseOffset;
		}
	}
}
