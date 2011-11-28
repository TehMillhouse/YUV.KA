using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YuvKA
{
	public class PipelineDriver
	{
		public PipelineGraph Graph
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public Task RenderFrame(int frameIndex) 
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<Frame> Render(INode node, CancellationToken token)
		{
			throw new System.NotImplementedException();
		}
	}
}
