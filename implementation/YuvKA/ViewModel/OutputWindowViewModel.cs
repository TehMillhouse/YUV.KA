using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public abstract class OutputWindowViewModel : Screen, IHandle<TickRenderedMessage>
	{
		public OutputWindowViewModel(Node nodeModel, Node.Output outputModel)
		{
			NodeModel = nodeModel;
			OutputModel = outputModel;
			IoC.Get<IEventAggregator>().Subscribe(this);
		}

		public Node NodeModel { get; private set; }
		public Node.Output OutputModel { get; private set; }

		public void CloseWindow()
		{
			IoC.Get<IEventAggregator>().Publish(new ClosedMessage(this));
			IoC.Get<IEventAggregator>().Unsubscribe(this);
		}

		public virtual void Handle(TickRenderedMessage message)
		{
		}

		public class ClosedMessage
		{
			public ClosedMessage(OutputWindowViewModel closedWindow) { Window = closedWindow; }
			public OutputWindowViewModel Window { get; private set; }
		}
	}
}
