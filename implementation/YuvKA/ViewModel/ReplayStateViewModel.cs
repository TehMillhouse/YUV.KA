using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
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
			if (Parent.Model.Speed > 1)
				Parent.Model.Speed /= 2;
		}

		public void PlayPause()
		{
			if (Parent.Model.CurrentTick == Parent.Model.Graph.TickCount)
				Stop();

			if (!IsPlaying) {
				if (Parent.Model.Start(Parent.OpenWindows.Select(w => w.NodeModel)))
					IsPlaying = !IsPlaying;
			}
			else {
				Parent.Model.Stop();
				IsPlaying = !IsPlaying;
			}
		}

		public void Stop()
		{
			Parent.Model.CurrentTick = 0;
			IsPlaying = false;
			Parent.Model.Stop();
		}

		public void Faster()
		{
			Parent.Model.Speed *= 2;
		}

		public void Handle(TickRenderedMessage message)
		{
			if (Parent.Model.CurrentTick == Parent.Model.Graph.TickCount - 1)
				IsPlaying = false;
		}
	}
}
