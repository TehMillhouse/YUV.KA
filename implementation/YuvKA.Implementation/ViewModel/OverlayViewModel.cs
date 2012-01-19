using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class OverlayViewModel : OutputWindowViewModel
	{
		public OverlayViewModel(Node nodeModel)
			: base(nodeModel)
		{
		}

		public new OverlayNode NodeModel
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
