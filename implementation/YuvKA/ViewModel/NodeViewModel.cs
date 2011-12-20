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
		public MainViewModel Parent { get { throw new NotImplementedException(); } }

		public void SaveNodeOutput(Node.Output output)
		{
			throw new System.NotImplementedException();
		}

		public void ShowNodeOutput(Node.Output output)
		{
			//Parent.OpenWindow(new VideoOutputViewModel { Output = output });
		}
	}
}
