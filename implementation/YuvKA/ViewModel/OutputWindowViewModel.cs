using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public abstract class OutputWindowViewModel : PropertyChangedBase, IHandle<TickRenderedMessage>
	{
		public Node NodeModel { get; protected set; }

		public virtual void Handle(TickRenderedMessage message)
		{
		}
	}
}
