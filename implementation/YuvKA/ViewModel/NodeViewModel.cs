using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class NodeViewModel
	{
		public NodeType NodeType { get { throw new NotImplementedException(); } }
		public Node NodeModel { get { throw new NotImplementedException(); } }
		public double X { get; set; }
		public double Y { get; set; }

		public void SaveNodeOutput(Node.Output output)
		{
			throw new System.NotImplementedException();
		}
	}
}
