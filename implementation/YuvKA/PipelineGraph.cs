using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class PipelineGraph
	{
		public IList<Tuple<INode, INode>> Edges
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public IList<INode> Nodes
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public int FrameCount
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public bool AddEdge(Tuple<INode, INode> edge)
		{
			throw new System.NotImplementedException();
		}
	}
}
