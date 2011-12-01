using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class NodeViewModel
	{
		public NodeTypeViewModel NodeType { get; private set; }
		public Node NodeModel { get; private set; }
		public double X { get; set; }
		public double Y { get; set; }

		public void SaveNodeOutput(Node.Output output)
		{
			throw new System.NotImplementedException();
		}
	}
}
