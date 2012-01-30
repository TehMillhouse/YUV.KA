using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class DiagramViewModel : OutputWindowViewModel, IHandle<DeleteGraphControlMessage>, IHandle<GraphTypeChosenMessage>
	{
		private ObservableCollection<LineGraphViewModel> lineGraphs;

		public DiagramViewModel(Node nodeModel)
			: base(nodeModel, null)
		{
		}

		private ICommand add;

		public ICommand Add
		{
			get { return add ?? (add = new RelayCommand(param => AddGraphControl(), param => true)); }
		}

		private void AddGraphControl()
		{
			var graph = new DiagramGraph { Video = NodeModel.Inputs[Videos.IndexOf(ChosenVideo)] };
			var graphControl = new GraphControl { Video = ChosenVideo, Types = new ObservableCollection<Tuple<string, IGraphType>>(Types), DisplayTypes = new ObservableCollection<Tuple<string, IGraphType>>(Types), Graph = graph };
			graphControl.setDisplayTypes();
			if (Reference.Item2 != null)
				graphControl.ReferenceSet = true;
			else
				Reference = new Tuple<string, Node.Input>("Video0", NodeModel.Inputs[0]);
			GraphControls.Add(graphControl);
			//AddGraph(graph);
			//AddLineGraphViewModel(graphControl);
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
				return (from IGraphType type in IoC.GetAllInstances(typeof(IGraphType)) select new Tuple<string, IGraphType>(type.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, type)).ToList();
			}
		}

		public Tuple<string, Node.Input> Reference
		{
			get { return new Tuple<string, Node.Input>("Video" + NodeModel.Inputs.IndexOf(NodeModel.ReferenceVideo), NodeModel.ReferenceVideo); }
			set
			{
				NodeModel.ReferenceVideo = NodeModel.Inputs[Videos.IndexOf(value)];
				foreach (var graphControl in GraphControls) {
					graphControl.ReferenceSet = true;
					graphControl.setDisplayTypes();
				}
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

		public ObservableCollection<GraphControl> GraphControls
		{
			get
			{
				if (addedGraphViews != null)
					return addedGraphViews;
				addedGraphViews = new ObservableCollection<GraphControl>();

				return addedGraphViews;
			}
			set { addedGraphViews = value; }
		}

		public void AddLineGraphViewModel(GraphControl graphControl)
		{
			var line = new LineGraphViewModel();
			var inDat = new ObservableCollection<Point>();


			var index = 0;
			foreach (var d in graphControl.Graph.Data) {
				inDat.Add(new Point(d.Key, d.Value));
				index++;
			}

			var data = new ObservableDataSource<Point>(inDat);
			data.SetXMapping(k => k.X);
			data.SetYMapping(k => k.Y);
			var ds = new CompositeDataSource(data);
			line.PointDataSource = ds;
			if (graphControl.ChosenType != null) {
				line.Name = graphControl.Video.Item1 + graphControl.ChosenType.Item1;
				line.Name = line.Name.Replace("-", "");
			} else {
				line.Name = graphControl.Video.Item1;
			}
			line.Color = graphControl.LineColor;
			line.EntityId = Guid.NewGuid();
			line.LineAndMarker = false;
			line.Thickness = 1;
			LineGraphs.Add(line);
		}

		public void DeleteGraph(DiagramGraph graph)
		{
			NodeModel.Graphs.Remove(graph);
		}

		public void AddGraph(DiagramGraph graph)
		{
			NodeModel.Graphs.Add(graph);
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
			LineGraphs[NodeModel.Graphs.IndexOf(message.GraphControltoDelete.Graph)].PointDataSource = new CompositeDataSource();
			LineGraphs.RemoveAt(NodeModel.Graphs.IndexOf(message.GraphControltoDelete.Graph));
			GraphControls.Remove(message.GraphControltoDelete);
			DeleteGraph(message.GraphControltoDelete.Graph);
		}

		private ObservableDataSource<Point> ConvertToDataSource(IEnumerable<KeyValuePair<int, double>> data)
		{
			var inData = new ObservableCollection<Point>();
			foreach (var d in data) {
				inData.Add(new Point(d.Key, d.Value));
			}
			var outData = new ObservableDataSource<Point>(inData);
			outData.SetXMapping(k => k.X);
			outData.SetYMapping(k => k.Y);
			return outData;
		}

		public override void Handle(TickRenderedMessage message)
		{
			for (int index = 0; index < LineGraphs.Count; index++) {
				var l = LineGraphs[index];
				l.Color = GraphControls[index].LineColor;
				l.PointDataSource = new CompositeDataSource(ConvertToDataSource(NodeModel.Graphs[index].Data));
				LineGraphs[index] = l;
			}
		}

		public void Handle(GraphTypeChosenMessage message)
		{
			AddGraph(message.GraphControl.Graph);
			AddLineGraphViewModel(message.GraphControl);
		}
	}
}