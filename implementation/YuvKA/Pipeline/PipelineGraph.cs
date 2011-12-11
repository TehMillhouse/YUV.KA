using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.Pipeline
{
	[DataContract]#
	public class PipelineGraph
	{

		[DataMember]
		public IList<Node> Nodes
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[DataMember]
		public int FrameCount
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public bool AddEdge(Node.Output source, Node.Input sink)
		{
			throw new System.NotImplementedException();
		}
	}
}
