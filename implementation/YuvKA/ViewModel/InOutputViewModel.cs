using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class InOutputViewModel : ViewAware
	{
		public InOutputViewModel(object model, NodeViewModel parent)
		{
			Model = model;
			Parent = parent;
		}

		public object Model { get; private set; }
		public NodeViewModel Parent { get; private set; }
		public bool IsFake { get { return Model == null; } }
	}
}