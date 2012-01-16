using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class InOutputViewModel : ViewAware
	{
		public InOutputViewModel(object model)
		{
			Model = model;
		}

		public object Model { get; private set; }
		public bool IsFake { get { return Model == null; } }
	}
}