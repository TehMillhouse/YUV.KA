using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YuvKA
{
	public static class PipelineDriver
	{
		public static Task RenderFrame(PipelineGraph graph, int frameIndex) 
		{
			throw new System.NotImplementedException();
		}

		public static IEnumerable<Frame> RenderAllTheFrames(PipelineGraph graph, Node node, CancellationToken token)
		{
			throw new System.NotImplementedException();
		}
	}
}
