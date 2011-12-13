using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public abstract class OutputWindowViewModel : IHandle<FrameRenderedMessage>
	{
		public Node NodeModel { get; }

		public virtual void Handle(FrameRenderedMessage message)
		{
		}
	}
}
