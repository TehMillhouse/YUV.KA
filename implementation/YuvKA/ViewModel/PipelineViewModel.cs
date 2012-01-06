using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class PipelineViewModel
	{
		public PipelineViewModel(MainViewModel parent)
		{
			Parent = parent;
			Nodes = Parent.Model.Graph.Nodes.Select(n => new NodeViewModel(n, Parent)).ToList();
		}

		public MainViewModel Parent { get; private set; }
		public IList<NodeViewModel> Nodes { get; private set; }
		public IEnumerable<EdgeViewModel> Edges { get; private set; }

		public void Drop(DragEventArgs e)
		{
			var type = (NodeType)e.Data.GetData(typeof(NodeType));
			var node = (Node)Activator.CreateInstance(type.Type);
			Parent.Model.Graph.Nodes.Add(node);
		}
	}
}
