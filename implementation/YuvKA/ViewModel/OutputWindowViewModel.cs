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
		}

		public Node NodeModel { get; private set; }
		public Node.Output OutputModel { get; private set; }

		public virtual void Handle(TickRenderedMessage message)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			IoC.Get<IEventAggregator>().Subscribe(this);
		}

		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
			IoC.Get<IEventAggregator>().Publish(new ClosedMessage(this));
			IoC.Get<IEventAggregator>().Unsubscribe(this);
		}

		public class ClosedMessage
		{
			public ClosedMessage(OutputWindowViewModel closedWindow) { Window = closedWindow; }
			public OutputWindowViewModel Window { get; private set; }
		}
	}
}
