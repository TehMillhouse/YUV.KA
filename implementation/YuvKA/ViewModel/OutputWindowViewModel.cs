﻿using System.Windows;
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

		public void CloseWindow()
		{
			IoC.Get<IEventAggregator>().Publish(new ClosedMessage(this));
		}

		public virtual void Handle(TickRenderedMessage message)
		{
		}

		public class ClosedMessage {
			public ClosedMessage(OutputWindowViewModel closedWindow) { Window = closedWindow; }
			public OutputWindowViewModel Window { get; private set; }
		}
	}
}
