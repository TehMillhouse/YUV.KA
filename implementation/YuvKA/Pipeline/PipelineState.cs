using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Caliburn.Micro;

namespace YuvKA.Pipeline
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Long-living, rarely instantiated class")]
	[DataContract]
	public class PipelineState : PropertyChangedBase
	{
		CancellationTokenSource cts;
		DateTimeOffset? lastTick;
		int currentTick;

		public PipelineState()
		{
			Graph = new PipelineGraph();
			Driver = new PipelineDriver();
		}

		[DataMember]
		public int CurrentTick
		{
			get { return currentTick; }
			set
			{
				if (value != currentTick) {
					currentTick = value;
					NotifyOfPropertyChange(() => CurrentTick);
				}
			}
		}

		public PipelineDriver Driver { get; private set; }

		/// <summary>
		/// Replay speed in frames per second
		/// </summary>
		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public PipelineGraph Graph { get; private set; }

		[Import]
		IEventAggregator Events { get; set; }

		public bool Start(IEnumerable<Node> outputNodes)
		{
			return RenderTicks(outputNodes, Graph.TickCount - CurrentTick, isPreviewFrame: false);
		}

		public void Stop()
		{
			if (cts != null)
				cts.Cancel();
		}

		public void RenderTick(IEnumerable<Node> outputNodes, bool isPreviewFrame)
		{
			RenderTicks(outputNodes, 1, isPreviewFrame);
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			Driver = new PipelineDriver();
		}

		private bool RenderTicks(IEnumerable<Node> outputNodes, int tickCount, bool isPreviewFrame)
		{
			if (!outputNodes.All(node => node.InputIsValid))
				return false;
			cts = new CancellationTokenSource();
			int precomputeCount = Graph.NumberOfFramesToPrecompute(outputNodes);
			Driver.RenderTicks(outputNodes, CurrentTick - precomputeCount, tickCount + precomputeCount, cts).Subscribe(dic => {
				if (lastTick.HasValue) {
					DateTimeOffset nextTick = lastTick.Value + TimeSpan.FromSeconds(1.0 / Speed);
					if (DateTimeOffset.Now < nextTick)
						Thread.Sleep(nextTick - lastTick.Value);
					lastTick = nextTick;
				}
				Events.Publish(new TickRenderedMessage(dic));
				if (!isPreviewFrame) {
					CurrentTick++;
				}
			});
			return true;
		}
	}
}
