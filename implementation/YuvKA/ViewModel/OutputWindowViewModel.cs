using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public abstract class OutputWindowViewModel : IHandle<TickRenderedMessage>
	{
		public Node NodeModel { get; private set; }

		public virtual void Handle(TickRenderedMessage message)
		{
		}
	}
}
