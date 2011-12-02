using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	public static class PipelineDriver
	{
		public static Task RenderFrame(PipelineGraph graph, IEnumerable<Node> startNodes, int frameIndex) 
		{
			//ILookup<Node, Task> tasks = graph.Nodes.ToLookup(node => new Task(() => node.ProcessFrame(int frameIndex)));
			throw new System.NotImplementedException();
		}

		public static IEnumerable<Frame> RenderAllTheFrames(PipelineGraph graph, Node node, CancellationToken token)
		{
			throw new System.NotImplementedException();
		}
	}
}
