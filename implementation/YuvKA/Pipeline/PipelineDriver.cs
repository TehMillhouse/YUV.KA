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
		public static Task RenderFrame(IEnumerable<Node> startNodes, int frameIndex) 
		{
			var tasks = new Dictionary<Node, Task>();
			IEnumerable<Task> startTasks = startNodes.Select(start => Visit(start, tasks, frameIndex));
			return Task.Factory.ContinueWhenAll(startTasks.ToArray(), _ => { });
		}

		private static Task Visit(Node node, Dictionary<Node, Task> tasks, int frameIndex)
		{
			Task result;
			if (!tasks.TryGetValue(node, out result)) {
				IEnumerable<Task> dependencies = node.Inputs.Select(i => Visit(i.Source.Node, tasks, frameIndex)); 
				tasks.Add(node, result = Task.Factory.ContinueWhenAll(dependencies.ToArray(), _ => node.ProcessFrame(frameIndex)));
			}
			return result;
		}

		public static IEnumerable<Frame> RenderAllTheFrames(Node.Output output, int frameCount, CancellationToken token)
		{
			for (int i = 0; i < frameCount; i++) {
				token.ThrowIfCancellationRequested();
				RenderFrame(new[] { output.Node }, i).Wait(token);
				yield return output.Buffer;
			}
		}
	}
}
