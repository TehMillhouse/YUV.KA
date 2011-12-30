using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YuvKA.Pipeline;

namespace YuvKA.Test.Pipeline
{
	public class PipelineDriverTest
	{
		/// <summary>
		/// RenderTick should be able to correctly process the following graph:
		///     [id]
		///    /    \
		/// [1]      [(x,y) => x+y] (hint: result is 2)
		///    \    /
		///     [id]
		/// </summary>
		[Fact]
		public void RenderTickProcessesDiamondGraph()
		{
			AnonIntNode start = new AnonIntNode(_ => 1);
			Node graph = new AnonIntNode(inputs => inputs[0] + inputs[1],
				new AnonIntNode(inputs => inputs[0], start),
				new AnonIntNode(inputs => inputs[0], start)
			);

			Assert.Equal(2, new PipelineDriver().RenderTick(new[] { graph }, 0, new CancellationToken()).Result[graph.Outputs[0]].Size.Width);
		}

		// From here on I'll limit the tests to RenderTicks because RenderTick is really not that interesting.

		/// <summary>
		/// RenderTicks should be able to correctly process the following graph:
		///         [id]
		///        /    \
		/// [ticks]      [(x,y) => x+y]
		///        \    /
		///         [id]
		/// </summary>
		[Fact]
		public void RenderTicksProcessesDiamondGraph()
		{
			AnonIntNode start = new AnonIntNode((_, tick) => tick);
			AnonIntNode graph = new AnonIntNode(inputs => inputs[0] + inputs[1],
				new AnonIntNode(inputs => inputs[0], start),
				new AnonIntNode(inputs => inputs[0], start)
			);

			var tokenSource = new CancellationTokenSource();
			var frames = RenderTicksAnonIntNodes(new PipelineDriver(), graph, 0, tokenSource).Take(5).ToEnumerable();

			Assert.Equal(new[] { 0, 2, 4, 6, 8 }, frames.ToArray());
			// Token should be cancelled at this point by completing Observable.Take
			Assert.True(tokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
		}

		[Fact]
		public void RenderTicksProcessesLongGraph()
		{
			AnonIntNode graph = Enumerable.Range(0, 100).Aggregate(
				new AnonIntNode((inputs, tick) => tick),
				(node, _) => new AnonIntNode(i => i[0] + 1, node)
			);

			Assert.Equal(Enumerable.Range(100, 20).ToArray(), RenderTicksAnonIntNodes(new PipelineDriver(), graph, 0, new CancellationTokenSource()).Take(20).ToEnumerable().ToArray());
		}

		/// <summary>
		/// Invoking RenderTicks on graph1 should wait for the completion of graph0.
		/// </summary>
		[Fact]
		public void RenderTicksNonReentrant()
		{
			var driver = new PipelineDriver();
			bool firstInvocationCompleted = false;

			Node graph1 = new AnonymousNode(() => Assert.True(firstInvocationCompleted));

			Task task = null;
			Node graph0 = new AnonymousNode(() => {
				task = driver.RenderTicks(new[] { graph1 }, 0, new CancellationTokenSource()).Take(1).ToTask();
				Thread.Sleep(1000);
				firstInvocationCompleted = true;
			});

			task.Wait();
			driver.RenderTicks(new[] { graph0 }, 0, new CancellationTokenSource()).First();
		}

		/// <summary>
		/// No Process method should be invoked after signalling the token.
		/// </summary>
		[Fact]
		public void RenderTicksEarlyCancellation()
		{
			var tokenSource = new CancellationTokenSource();
			bool shouldThrow = false;

			Node graph = new AnonymousNode(
				() => { throw new InvalidOperationException(); },
				new AnonymousNode(() => { if (shouldThrow) tokenSource.Cancel(); })
			);

			// If the first node doesn't cancel, the second one should throw the exception
			var ex = Assert.Throws<AggregateException>(() => new PipelineDriver().RenderTicks(new[] { graph }, 0, new CancellationTokenSource()).Last());
			Assert.IsType<InvalidOperationException>(ex.GetBaseException());

			// If the first node does cancel, the second one shouldn't be processed and no tick should be returned
			shouldThrow = true;
			Assert.Equal(0, new PipelineDriver().RenderTicks(new[] { graph }, 0, tokenSource).Count().Last());
		}

		// Render a graph of anonymous int-returning nodes
		IObservable<int> RenderTicksAnonIntNodes(PipelineDriver driver, AnonIntNode startNode, int tick, CancellationTokenSource tokenSource)
		{
			return driver.RenderTicks(new[] { startNode }, tick, tokenSource).Select(dic => dic[startNode.Outputs[0]].Size.Width);
		}
	}
}
