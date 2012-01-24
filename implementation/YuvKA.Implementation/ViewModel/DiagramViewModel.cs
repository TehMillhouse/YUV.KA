using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class DiagramViewModel : OutputWindowViewModel, INotifyPropertyChanged, IHandle<DeleteGraphControlMessage>
	{
		private ObservableCollection<LineGraphViewModel> lineGraphs;
		public DiagramViewModel(Node nodeModel)
			: base(nodeModel)
		{
		}

		private RelayCommand add;

		public ICommand Add
		{
			get { return add ?? (add = new RelayCommand(param => AddGraphControl(), param => true)); }
		}

		private void AddGraphControl()
		{
			var random = new Random();
			GraphControls.Add(new GraphControl { Video = ChosenVideo, Types = Types, Color = new SolidColorBrush(Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256))) });
		}

		public int del()
		{

			return 0;
		}

		public ObservableCollection<LineGraphViewModel> LineGraphs
		{
			get { return lineGraphs ?? (lineGraphs = new ObservableCollection<LineGraphViewModel>()); }
		}

		public new DiagramNode NodeModel { get { return (DiagramNode)base.NodeModel; } }


		public IEnumerable<Tuple<string, IGraphType>> Types
		{
			get
			{
				
				ICollection<System.Tuple<string, IGraphType>> graphTypes = new List<System.Tuple<string, IGraphType>>();
				/* Does not work at the moment. */
				//foreach (IGraphType type in IoC.GetAllInstances(typeof(IGraphType))) {
				//    graphTypes.Add(new System.Tuple<string, IGraphType>(type.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, type));
				//}
				//return graphTypes;

				graphTypes.Add(new Tuple<string, IGraphType>(typeof(IntraBlockFrequency).GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, typeof(IntraBlockFrequency) as IGraphType));
				graphTypes.Add(new Tuple<string, IGraphType>(typeof(PeakSignalNoiseRatio).GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, typeof(PeakSignalNoiseRatio) as IGraphType));
				graphTypes.Add(new Tuple<string, IGraphType>(typeof(PixelDiff).GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, typeof(PixelDiff) as IGraphType));
				return graphTypes;
			}
		}

		public Tuple<string, Node.Input> Reference
		{
			get { return new Tuple<string, Node.Input>("Video" + NodeModel.Inputs.IndexOf(NodeModel.ReferenceVideo), NodeModel.ReferenceVideo); }
			set
			{
				NodeModel.ReferenceVideo = value.Item2;
			}
		}

		public ObservableCollection<Tuple<string, Node.Input>> Videos
		{
			get
			{
				var videos = new ObservableCollection<Tuple<string, Node.Input>>();
				int index = 0;
				foreach (Node.Input i in NodeModel.Inputs) {
					videos.Add(new Tuple<string, Node.Input>("Video" + index, i));
					index++;
				}
				return videos;
			}
		}

		public List<Tuple<string, IGraphType>> ChosenTypes
		{
			get
			{
				return (from IGraphType type in IoC.GetAllInstances(typeof(IGraphType)) select new Tuple<string, IGraphType>(type.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, type)).ToList();
			}
		}

		public Tuple<string, Node.Input> ChosenVideo { get; set; }

		public ICommand Delete()
		{
			return null;
		}

		public ObservableCollection<GraphControl> GraphControls
		{
			get
			{
				if (addedGraphViews != null) return addedGraphViews;
				addedGraphViews = new ObservableCollection<GraphControl>();

				return addedGraphViews;
			}
			set { addedGraphViews = value; }
		}

		public void DeleteGraph(DiagramGraph graph)
		{
			NodeModel.Graphs.Remove(graph);
		}

		public void AddGraph(Node.Input video)
		{
			var newGraph = new DiagramGraph { Video = video };
			NodeModel.Graphs.Add(newGraph);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private ObservableCollection<GraphControl> addedGraphViews;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				var e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}

		public void Handle(DeleteGraphControlMessage message)
		{
			GraphControls.Remove(message.GraphViewtoDelete);
		}
	}
}