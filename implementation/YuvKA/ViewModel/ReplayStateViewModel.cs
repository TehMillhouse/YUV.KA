using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Manages the current state of the replay.
	/// </summary>
	[Export]
	public class ReplayStateViewModel : PropertyChangedBase, IHandle<TickRenderedMessage>
	{
		bool isPlaying;

		public ReplayStateViewModel()
		{
			IoC.Get<IEventAggregator>().Subscribe(this);
		}

		public bool IsPlaying
		{
			get { return isPlaying; }
			set
			{
				isPlaying = value;
				NotifyOfPropertyChange(() => IsPlaying);
			}
		}

		[Import]
		public MainViewModel Parent { get; private set; }

		public void Slower()
		{
			if (Parent.Model.Speed > 5)
				Parent.Model.Speed -= 5;
			else
				Parent.Model.Speed = 1;
		}

		/// <summary>
		/// Pauses the replay or resumes if already paused.
		/// </summary>
		public void PlayPause()
		{
			if (Parent.Model.CurrentTick == Parent.Model.Graph.TickCount)
				Stop();

			if (!IsPlaying) {
				if (Parent.Model.Start(GetNodesToProcess()))
					IsPlaying = !IsPlaying;
			}
			else {
				Parent.Model.Stop();
				IsPlaying = !IsPlaying;
			}
		}

		/// <summary>
		/// Pauses the replay and resets the replay position to the start of the video.
		/// </summary>
		public void Stop()
		{
			Parent.Model.CurrentTick = 0;
			IsPlaying = false;
			Parent.Model.Stop();
		}

		public void Faster()
		{
			Parent.Model.Speed += 5;
		}

		void IHandle<TickRenderedMessage>.Handle(TickRenderedMessage message)
		{
			if (Parent.Model.CurrentTick == Parent.Model.Graph.TickCount - 1)
				IsPlaying = false;
		}

		IEnumerable<Node> GetNodesToProcess()
		{
			return Parent.OpenWindows.Select(w => w.NodeModel)
				.Concat(Parent.Model.Graph.Nodes.Where(n => n.ProcessNodeInBackground));
		}
	}
}
