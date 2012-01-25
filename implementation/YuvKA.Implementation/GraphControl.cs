using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.Implementation
{
	public class GraphControl : INotifyPropertyChanged
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

		public DiagramGraph Graph { get; set; }

		public IEnumerable<Tuple<string, IGraphType>> Types { get; set; }

		private Tuple<string, IGraphType> chosenType;

		public Tuple<string, IGraphType> ChosenType
		{
			get { return chosenType; }
			set
			{
				chosenType = value;
				Graph.Type = value.Item2;
			}
		}

		public Color LineColor
		{
			get
			{
				if (ChosenType != null) {
					if (ChosenType.Item2 == (IGraphType)typeof(IntraBlockFrequency))
						return Color.FromRgb(255, 0, 0);
					if (ChosenType.Item2 == (IGraphType)typeof(PeakSignalNoiseRatio))
						return Color.FromRgb(0, 255, 0);
					if (ChosenType.Item2 == (IGraphType)typeof(PixelDiff))
						return Color.FromRgb(0, 0, 255);
				}
				else {
					return Color.FromRgb(0, 0, 0);
				}
				return Color.FromRgb(0, 0, 0);
			}
		}

		public SolidColorBrush GraphColor { get { return new SolidColorBrush(LineColor); } }

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler == null)
				return;
			var e = new PropertyChangedEventArgs(propertyName);
			handler(this, e);
		}
	}
}