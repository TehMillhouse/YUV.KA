using System.Linq;

namespace YuvKA.ViewModel
{
	public class ReplayStateViewModel
	{
		public ReplayStateViewModel(MainViewModel parent)
		{
			Parent = parent;
		}

		public bool IsPlaying { get; private set; }
		public MainViewModel Parent { get; private set; }

		public void Slower()
		{
			if (Parent.Model.Speed > 1)
				Parent.Model.Speed /= 2;
		}

		public void PlayPause()
		{
			if (Parent.Model.CurrentTick == Parent.Model.Graph.TickCount - 1)
				Stop();

			if (IsPlaying)
				Parent.Model.Start(Parent.OpenWindows.Select(w => w.NodeModel));
			else
				Parent.Model.Stop();
			IsPlaying = !IsPlaying;
		}

		public void Stop()
		{
			Parent.Model.CurrentTick = 0;
			IsPlaying = false;
		}

		public void Faster()
		{
			Parent.Model.Speed *= 2;
		}
	}
}
