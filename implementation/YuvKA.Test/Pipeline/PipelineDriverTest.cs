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
	public class PipelineDriverTest : IDisposable
	{
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

		[Fact]
		public void RenderTicksStressTest()
		{
			for (int i = 0; i < 100; i++)
				RenderTicksProcessesLongGraph();
		}

		[Fact]
		public void RenderTicksClosesObservable()
		{
			var cts = new CancellationTokenSource();
			Node graph = new AnonymousNode(() => { cts.Cancel(); cts.Token.ThrowIfCancellationRequested(); });

			Assert.Equal(0, new PipelineDriver().RenderTicks(new[] { graph }, tokenSource: cts).Count().Last());
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}


		[Fact]
		public void RenderTicksPropagatesExceptions()
		{
			Node graph = new AnonymousNode(() => { throw new InvalidOperationException(); });

			var ex = Assert.Throws<AggregateException>(() => new PipelineDriver().RenderTicks(new[] { graph }).Last());
			Assert.IsType<InvalidOperationException>(ex.Flatten().InnerException);
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
				task = driver.RenderTicks(new[] { graph1 }, tickCount: 1).ToTask();
				Thread.Sleep(1000);
				firstInvocationCompleted = true;
			});

			driver.RenderTicks(new[] { graph0 }).First();
			task.Wait();
		}

		/// <summary>
		/// No Process method should be invoked after signalling the token.
		/// </summary>
		[Fact]
		public void RenderTicksEarlyCancellation()
		{
			var tokenSource = new CancellationTokenSource();

			Node graph = new AnonymousNode(
				() => { throw new InvalidOperationException(); },
				new AnonymousNode(() => tokenSource.Cancel())
			);

			Assert.Equal(0, new PipelineDriver().RenderTicks(new[] { graph }, tokenSource: tokenSource).Count().Last());
			// Looks like the InvalidOperationException hasn't been thrown. Yay!
		}



		// Catch free-flying non-observed Tasks
		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		// Render a graph of anonymous int-returning nodes
		IObservable<int> RenderTicksAnonIntNodes(PipelineDriver driver, AnonIntNode startNode, int tick, CancellationTokenSource tokenSource)
		{
			return driver.RenderTicks(new[] { startNode }, tokenSource: tokenSource).Select(dic => dic[startNode.Outputs[0]].Size.Width);
		}
	}
}
