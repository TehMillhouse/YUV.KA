using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Caliburn.Micro;

namespace YuvKA.Pipeline
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Long-living, rarely instantiated class")]
	[DataContract]
	public class PipelineState
	{
		PipelineDriver driver = new PipelineDriver();
		CancellationTokenSource cts;
		DateTimeOffset? lastTick;

		public PipelineState()
		{
			Graph = new PipelineGraph();
		}

		[DataMember]
		public int CurrentTick { get; set; }

		/// <summary>
		/// Replay speed in frames per second
		/// </summary>
		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public PipelineGraph Graph { get; private set; }

		[Import]
		IEventAggregator Events { get; set; }

		public void Start(IEnumerable<Node> outputNodes)
		{
			RenderTicks(outputNodes, Graph.TickCount);
		}

		public void Stop()
		{
			if (cts != null)
				cts.Cancel();
		}

		public void RenderTick(IEnumerable<Node> outputNodes)
		{
			RenderTicks(outputNodes, tickCount: 1);
		}

		private void RenderTicks(IEnumerable<Node> outputNodes, int tickCount)
		{
			cts = new CancellationTokenSource();
			driver.RenderTicks(outputNodes, CurrentTick, tickCount, cts).ForEach(dic => {
				if (lastTick.HasValue) {
					DateTimeOffset nextTick = lastTick.Value + TimeSpan.FromSeconds(1.0 / Speed);
					if (DateTimeOffset.Now < nextTick)
						Thread.Sleep(nextTick - lastTick.Value);
					lastTick = nextTick;
				}
				Events.Publish(new TickRenderedMessage(dic));
				CurrentTick++;
			});
		}
	}
}
