using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YuvKA.Pipeline;
using System.Collections.ObjectModel;

namespace YuvKA.ViewModel
{
	public class PipelineViewModel
	{
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
			Parent.Model.Graph.Nodes.Add(node);
			Nodes.Add(new NodeViewModel(node, Parent));
		}
	}
}
