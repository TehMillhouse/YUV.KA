using System;
using System.Collections.Generic;
using System.Windows;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class PipelineViewModel
	{
		PipelineGraph pipelineGraph;

		public PipelineViewModel(PipelineGraph pipelineGraph)
		{
			this.pipelineGraph = pipelineGraph;
		}

		public IList<NodeViewModel> Nodes { get; private set; }
		public IEnumerable<EdgeViewModel> Edges { get; private set; }

		public void Drop(DragEventArgs e)
		{
			var type = (NodeType)e.Data.GetData(typeof(NodeType));
			var node = (Node)Activator.CreateInstance(type.Type);
			pipelineGraph.Nodes.Add(node);
		}
	}
}
