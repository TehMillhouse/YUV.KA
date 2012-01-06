using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class VideoOutputViewModel : OutputWindowViewModel
	{
		public VideoOutputViewModel(Node.Output output)
		{
			Output = output;
		}

		public Node.Output Output { get; private set; }
	}
}
