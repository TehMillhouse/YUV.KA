using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.Implementation
{
	public class GraphControl
	{
		[Import]
		IEventAggregator Events { get; set; }

		public GraphControl()
		{
			Events = IoC.Get<IEventAggregator>();
		}

		private RelayCommand delete;

		public ICommand Delete
		{
			get
			{
				if (this.delete == null) {
					this.delete = new RelayCommand(param => this.Del(), param => true);
				}
				return this.delete;
			}
		}

		public void Del()
		{
			Events.Publish(new DeleteGraphControlMessage(this));
		}

		public Tuple<string, Node.Input> Video { get; set; }

		public IEnumerable<Tuple<string, IGraphType>> Types { get; set; }

		public Tuple<string, IGraphType> ChosenType { get; set; }

		public SolidColorBrush Color { get; set; }

	}
}