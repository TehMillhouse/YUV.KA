using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
		int currentTick;
		int speed;

		public PipelineState()
		{
			Graph = new PipelineGraph();
			Driver = new PipelineDriver();
			Speed = 30;
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
		public int Speed
		{
			get
			{
				return speed;
			}
			set
			{
				speed = value;
				NotifyOfPropertyChange(() => Speed);
			}
		}

		public int ActualSpeed { get; private set; }

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
		new void OnDeserialized(StreamingContext context)
		{
			Driver = new PipelineDriver();
		}

		private bool RenderTicks(IEnumerable<Node> outputNodes, int? tickCount, bool isPreviewFrame)
		{
			if (!outputNodes.All(node => node.InputIsValid))
				return false;
			cts = new CancellationTokenSource();
			int precomputeCount = Graph.NumberOfFramesToPrecompute(outputNodes);
			Queue<DateTimeOffset> ticks = new Queue<DateTimeOffset>();
			int fpsWindow = 5;

			Driver.RenderTicks(outputNodes, CurrentTick - precomputeCount, tickCount + precomputeCount, cts.Token).Subscribe(dic => {
				DateTimeOffset now = DateTimeOffset.Now;
				if (ticks.Count == fpsWindow) {
					DateTimeOffset oldTick = ticks.Dequeue();
					DateTimeOffset nextTick = oldTick + TimeSpan.FromSeconds((double)fpsWindow / Speed);
					if (now < nextTick) {
						Thread.Sleep(nextTick - now);
						now = DateTimeOffset.Now;
					}
					ActualSpeed = (int)(fpsWindow / (DateTimeOffset.Now - oldTick).TotalSeconds);
					NotifyOfPropertyChange(() => ActualSpeed);
				}
				ticks.Enqueue(now);

				Events.Publish(new TickRenderedMessage(dic));
				if (!isPreviewFrame && !cts.IsCancellationRequested) {
					CurrentTick++;
				}
			}, onError: e => Execute.OnUIThread(() => { throw e; }));
			return true;
		}
	}
}
