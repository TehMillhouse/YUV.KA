using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using Point = System.Windows.Point;

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
			var graphControl = new GraphControl { Video = ChosenVideo, Types = new ObservableCollection<Tuple<string, IGraphType>>(Types), DisplayTypes = new ObservableCollection<Tuple<string, IGraphType>>(Types), Graph = graph, TypeColors = TypeColors, LineColors = LineColors};
			graphControl.setDisplayTypes();
			if (Reference.Item2 != null)
				graphControl.ReferenceSet = true;
			else
				Reference = new Tuple<string, Node.Input>("Video0", NodeModel.Inputs[0]);
			GraphControls.Add(graphControl);
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
				if (graphControls != null)
					return graphControls;
				graphControls = new ObservableCollection<GraphControl>();

				return graphControls;
			}
			set { graphControls = value; }
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
		private ObservableCollection<GraphControl> graphControls;

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
			if (message.GraphControltoDelete.ChosenType != null)
			{
				LineGraphs[NodeModel.Graphs.IndexOf(message.GraphControltoDelete.Graph)].PointDataSource = new CompositeDataSource();
			LineGraphs.RemoveAt(NodeModel.Graphs.IndexOf(message.GraphControltoDelete.Graph));
			}
			
			GraphControls.Remove(message.GraphControltoDelete);
			DeleteGraph(message.GraphControltoDelete.Graph);
		}

		private static ObservableDataSource<Point> ConvertToDataSource(IEnumerable<KeyValuePair<int, double>> data)
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

		private List<System.Windows.Media.Color> typeColors;

		public List<System.Windows.Media.Color> TypeColors
		{
			get
			{
				if (typeColors == null) {
					var random = new Random();
					var randomColors = new List<Color>();
					var randomColorsMedia = new List<System.Windows.Media.Color>();
					var newColor = new Color();
					for (int i = 0; i < Types.Count(); i++)
					{
						do
						{
							newColor = Color.FromArgb(255, (byte) random.Next(256), (byte) random.Next(256), (byte) random.Next(256));
						} while (randomColors.FindIndex(color => color.GetHue().Equals(newColor.GetHue()) ) != -1);
						randomColors.Add(newColor);
						randomColorsMedia.Add(System.Windows.Media.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B));
					}
					typeColors = randomColorsMedia;
				}
				return typeColors;
			}
		}

		public List<Color> LineColors { get; set; }

		public System.Windows.Media.Color NewColor(Tuple<string, IGraphType> type)
		{
			var newColor = new Color();
			var typeCollection = new ObservableCollection<Tuple<string, IGraphType>>(Types);
			var baseColor = System.Drawing.Color.FromArgb(TypeColors[typeCollection.IndexOf(type)].A, TypeColors[typeCollection.IndexOf(type)].R, TypeColors[typeCollection.IndexOf(type)].G, TypeColors[typeCollection.IndexOf(type)].B);

			var h = baseColor.GetHue();

			var random = new Random();
			do
			{
				newColor = HslHelper.HslToRgb(h, random.NextDouble(), random.NextDouble());
			} while (LineColors.Contains(newColor));
			LineColors.Add(newColor);

			return System.Windows.Media.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
		}
	}
}