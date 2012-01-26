using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public abstract class OutputWindowViewModel : Screen, IHandle<TickRenderedMessage>
	{
		public OutputWindowViewModel(Node nodeModel)
		{
			NodeModel = nodeModel;
			IoC.Get<IEventAggregator>().Subscribe(this);
		}
		public Node NodeModel { get; private set; }

		public virtual void Handle(TickRenderedMessage message)
		{
		}
	}
}
