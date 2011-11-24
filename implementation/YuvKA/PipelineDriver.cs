using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace YuvKA
{
	public class PipelineDriver
	{
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

		public void Start(CancellationToken token)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<Frame> Render(INode node, CancellationToken token)
		{
			throw new System.NotImplementedException();
		}
	}
}
