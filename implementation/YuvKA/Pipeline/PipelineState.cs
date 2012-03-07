using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Caliburn.Micro;

namespace YuvKA.Pipeline
{
	/// <summary>
	/// Holding the model's complete state, the PipelineState class may be serialized to save all application data of a pipeline.
	/// Aside from the PipelineGraph it contains the replay state and can act on a PipelineDriver accordingly.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Long-living, rarely instantiated class")]
	[DataContract]
	public class PipelineState : PropertyChangedBase
	{
		CancellationTokenSource cts;
		Node[] nodesRendering;
		int currentTick;
		int speed;

		public PipelineState()
		{
			Graph = new PipelineGraph();
			Driver = new PipelineDriver();
			Speed = 30;
		}

		/// <summary>
		/// Gets or sets the next tick to be computed.
		/// </summary>
		[DataMember]
		public int CurrentTick
		{
			get { return currentTick; }
			set
			{
				if (value != currentTick) {
					Node[] nodesRendering = this.nodesRendering;
					Stop();
					currentTick = value;
					if (nodesRendering != null)
						Start(nodesRendering);
					NotifyOfPropertyChange(() => CurrentTick);
				}
			}
		}

		public bool IsPlaying
		{
			get { return nodesRendering != null; }
		}

		/// <summary>
		/// Gets the PipelineDriver instance used by this object.
		/// </summary>
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

		/// <summary>
		/// Gets the actual measured replay speed
		/// </summary>
		public int ActualSpeed { get; private set; }

		/// <summary>
		/// Gets the pipeline graph.
		/// </summary>
		[DataMember]
		public PipelineGraph Graph { get; private set; }

		/// <summary>
		/// Instructs the driver to render the given nodes, starting with CurrentTick. After each
		/// rendered tick a TickRenderedMessage will be thrown.
		/// </summary>
		/// <returns>True iff outputNodes can be rendered</returns>
		public bool Start(IEnumerable<Node> outputNodes)
		{
			nodesRendering = outputNodes.ToArray();
			if (RenderTicks(nodesRendering, Graph.TickCount - CurrentTick, isPreviewFrame: false)) {
				NotifyOfPropertyChange(() => IsPlaying);
				return true;
			}
			else {
				nodesRendering = null;
				return false;
			}
		}

		/// <summary>
		/// Stops the current rendering process, if any.
		/// </summary>
		public void Stop()
		{
			if (cts != null) {
				cts.Cancel();
				ActualSpeed = 0;
				NotifyOfPropertyChange(() => ActualSpeed);
				nodesRendering = null;
				NotifyOfPropertyChange(() => IsPlaying);
			}
		}

		/// <summary>
		/// Renders a single tick without advancing CurrentTick.
		/// </summary>
		public void RenderTick(IEnumerable<Node> outputNodes)
		{
			RenderTicks(outputNodes, 1, isPreviewFrame: true);
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

			Node[] nodesRendering = this.nodesRendering;
			cts = new CancellationTokenSource();
			int precomputeCount = Graph.NumberOfTicksToPrecompute(outputNodes);
			Queue<DateTime> ticks = new Queue<DateTime>();
			ticks.Enqueue(DateTime.Now);
			int windowSize = 5;

			Driver.RenderTicks(outputNodes, CurrentTick - precomputeCount, tickCount + precomputeCount, cts.Token)
				.Do(dic => {
					IoC.Get<IEventAggregator>().Publish(new TickRenderedMessage(dic));
					DateTime now = DateTime.Now;
					if (ticks.Count == windowSize) {
						// Compute "virtual" tick from average of tick window
						DateTime midTick = new DateTime((long)ticks.Average(t => t.Ticks));
						// tick count between virtual and current tick
						double midDelta = (windowSize + 1) / 2.0;
						DateTime nextTick = midTick + TimeSpan.FromSeconds(midDelta / Speed);
						if (now < nextTick) {
							Thread.Sleep(nextTick - now);
							if (cts.IsCancellationRequested)
								return;

							now = DateTime.Now;
						}
						ActualSpeed = (int)(midDelta / (now - midTick).TotalSeconds);
						NotifyOfPropertyChange(() => ActualSpeed);
						ticks.Dequeue();
					}
					ticks.Enqueue(now);
				})
				.ObserveOnDispatcher()
				.Subscribe(_ => {
					if (!isPreviewFrame) {
						currentTick++;
						NotifyOfPropertyChange(() => CurrentTick);
					}
				},
				onCompleted: () => {
					if (nodesRendering == this.nodesRendering)
						Stop();
				});
			return true;
		}
	}
}
