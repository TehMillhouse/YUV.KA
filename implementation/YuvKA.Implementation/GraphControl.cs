using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
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

		public void setDisplayTypes()
		{
			var newTypes = new ObservableCollection<Tuple<string, IGraphType>>();
			if (ReferenceSet == true) {
				foreach (var t in Types.Where(type => type.Item2.DependsOnReference))
					newTypes.Add(t);
			}
			if (Video.Item2.Source.Node.OutputHasLogfile == true) {
				foreach (var t in Types.Where(type => type.Item2.DependsOnLogfile))
					newTypes.Add(t);
			}
			DisplayTypes = newTypes;
			OnPropertyChanged("DisplayTypes");
		}

		public Tuple<string, Node.Input> Video { get; set; }

		public DiagramGraph Graph { get; set; }

		public ObservableCollection<Tuple<string, IGraphType>> Types { get; set; }

		public ObservableCollection<Tuple<string, IGraphType>> DisplayTypes { get; set; }

		private Tuple<string, IGraphType> chosenType;

		public Tuple<string, IGraphType> ChosenType
		{
			get { return chosenType; }
			set
			{
				if (chosenType == null) {
					chosenType = value;
					Graph.Type = value.Item2;
					setLineColor();
					Events.Publish(new GraphTypeChosenMessage(this));
				} else {
					chosenType = value;
					Graph.Type = value.Item2;
					setLineColor();
				}
			}
		}

		private List<Color> typeColors;

		public List<Color> TypeColors
		{
			get
			{
				if (typeColors == null) {
					var random = new Random();
					var randomColors = new List<Color>();
					for (int i = 0; i < Types.Count(); i++) {
						randomColors.Add(Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)));
					}
					typeColors = randomColors;
				}
				return typeColors;
			}
		}

		public void setLineColor()
		{
			LineColor = TypeColors[Types.IndexOf(ChosenType)];
			GraphColor = new SolidColorBrush(LineColor);
			OnPropertyChanged("LineColor");
			OnPropertyChanged("GraphColor");
		}

		public Color LineColor { get; set; }

		private bool referenceSet;
		public bool ReferenceSet
		{
			get { return referenceSet; }
			set
			{
				referenceSet = value;
				setDisplayTypes();
			}
		}

		public SolidColorBrush GraphColor { get; set; }

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