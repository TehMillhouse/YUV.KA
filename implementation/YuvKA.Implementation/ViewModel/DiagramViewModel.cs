using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using Point = System.Windows.Point;

namespace YuvKA.ViewModel.Implementation
{
	/// <summary>
	/// Provides the output window of a DiagramNode
	/// </summary>
	public class DiagramViewModel : OutputWindowViewModel
	{
		private ObservableCollection<LineGraphViewModel> lineGraphs;
		private ObservableCollection<GraphControl> graphControls;
		private List<System.Windows.Media.Color> typeColors;
		private List<Color> lineColors;

		/// <summary>
		/// Creates a new DiagramViewModel with the given DiagramNode.
		/// </summary>
		public DiagramViewModel(Node nodeModel)
			: base(nodeModel, null)
		{
		}

		/// <summary>
		/// Gets the Lines in the diagram corresponding to the graphs in the DiagramNode.
		/// </summary>
		public ObservableCollection<LineGraphViewModel> LineGraphs
		{
			get { return lineGraphs ?? (lineGraphs = new ObservableCollection<LineGraphViewModel>()); }
		}

		/// <summary>
		/// Gets the DiagramNode of this output window.
		/// </summary>
		public new DiagramNode NodeModel { get { return (DiagramNode)base.NodeModel; } }

		/// <summary>
		/// Gets the all available IGraphTypes.
		/// </summary>
		public IEnumerable<Tuple<string, IGraphType>> Types
		{
			get
			{
				return (from IGraphType type in IoC.GetAllInstances(typeof(IGraphType)) select new Tuple<string, IGraphType>(type.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, type)).ToList();
			}
		}

		/// <summary>
		/// Gets or sets the reference video of the DiagramNode.
		/// </summary>
		public Tuple<string, Node.Input> Reference
		{
			get { return new Tuple<string, Node.Input>("Video" + NodeModel.Inputs.IndexOf(NodeModel.ReferenceVideo), NodeModel.ReferenceVideo); }
			set
			{
				NodeModel.ReferenceVideo = NodeModel.Inputs[Videos.IndexOf(value)];
				// update the information of each GraphControl concerning the Reference.
				foreach (var graphControl in GraphControls) {
					graphControl.ReferenceSet = true;
					graphControl.ReferenceHasLogfile = value.Item2.Source.Node.OutputHasLogfile;
					// recalculate available types for this graphcontrol.
					graphControl.SetDisplayTypes();
				}
			}
		}

		/// <summary>
		/// Gets the inputvideos of the DiagramNode and adds an index to them.
		/// </summary>
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

		/// <summary>
		/// Gets or dets the video chosen by the user to be used for drawing a graph.
		/// </summary>
		public Tuple<string, Node.Input> ChosenVideo { get; set; }

		/// <summary>
		/// Gets the GraphControls of this DiagramViewModel.
		/// </summary>
		public ObservableCollection<GraphControl> GraphControls
		{
			get
			{
				if (graphControls != null)
					return graphControls;
				graphControls = new ObservableCollection<GraphControl>();

				return graphControls;
			}
		}

		/// <summary>
		/// Gets the list of all colors already used for drawing a line.
		/// </summary>
		public List<Color> LineColors
		{
			get { return lineColors ?? (lineColors = new List<Color>()); }
		}

		/// <summary>
		/// Gets the base colors of the types.
		/// </summary>
		public List<System.Windows.Media.Color> TypeColors
		{
			get
			{
				if (typeColors == null) {
					var random = new Random();
					var randomColors = new List<Color>();
					var randomColorsMedia = new List<System.Windows.Media.Color>();
					Color newColor;
					for (var i = 0; i < Types.Count(); i++) {
						do {
							newColor = Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
						} while (randomColors.FindIndex(color => IsInIntervall(color.GetHue(), newColor.GetHue(), 25.0)) != -1 || newColor.GetHue().Equals(0.0) || newColor.GetBrightness().Equals(1.0) || newColor.GetBrightness().Equals(0.0));
						randomColors.Add(newColor);
						randomColorsMedia.Add(System.Windows.Media.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B));
					}
					typeColors = randomColorsMedia;
				}
				return typeColors;
			}
		}

		/// <summary>
		/// Determines, whether a number is inside a given intervall
		/// </summary>
		public static bool IsInIntervall(double intervallCenter, double number, double intervallSize)
		{
			var difference = number - intervallCenter;
			return Math.Abs(difference).CompareTo(intervallSize) < 0;
		}

		/// <summary>
		/// Deletes a GraphControl and its  associated line and graph in the diagram.
		/// </summary>
		public void DeleteGraphControl(GraphControl graphControl)
		{
			if (graphControl.ChosenType != null) {
				var l = LineGraphs[GraphControls.IndexOf(graphControl)];
				// Datasource for line must be emptied, otherwise line will only hold.
				l.PointDataSource = new CompositeDataSource();
				LineGraphs[GraphControls.IndexOf(graphControl)] = l;
				LineGraphs.RemoveAt(GraphControls.IndexOf(graphControl));
			}
			GraphControls.Remove(graphControl);
			NodeModel.Graphs.Remove(graphControl.Graph);
		}

		/// <summary>
		/// Updates each line with the newly calculated frame once it is rendered.
		/// </summary>
		/// <param name="message">This parameter is unnused.</param>
		public override void Handle(TickRenderedMessage message)
		{
			for (var index = 0; index < LineGraphs.Count; index++) {
				var l = LineGraphs[index];
				//Change linecolor if the type of the graph has changed.
				l.Color = GraphControls[index].LineColor;
				l.PointDataSource = new CompositeDataSource(ConvertToDataSource(GraphControls[index].Graph.Data));
				LineGraphs[index] = l;
			}
		}

		/// <summary>
		/// Adds a Graph to the NodeModel and a line to the diagram from the given GraphControl.
		/// </summary>
		public void AddGraph(GraphControl graphControl)
		{
			NodeModel.Graphs.Add(graphControl.Graph);
			AddLineGraphViewModel(graphControl);
		}

		/// <summary>
		/// Adds a line to the diagram from the given graphControl
		/// </summary>
		public void AddLineGraphViewModel(GraphControl graphControl)
		{
			var line = new LineGraphViewModel();
			var inDat = new ObservableCollection<Point>(graphControl.Graph.Data.Select(datum => new Point(datum.Key, datum.Value)));
			var data = new ObservableDataSource<Point>(inDat);
			data.SetXMapping(k => k.X);
			data.SetYMapping(k => k.Y);
			var ds = new CompositeDataSource(data);
			line.PointDataSource = ds;
			if (graphControl.ChosenType != null) {
				line.Name = graphControl.Video.Item1 + graphControl.ChosenType.Item1;
				line.Name = line.Name.Replace("-", "");
			}
			line.Color = graphControl.LineColor;
			line.EntityId = Guid.NewGuid();
			line.LineAndMarker = false;
			line.Thickness = 1;
			LineGraphs.Add(line);
		}

		/// <summary>
		/// Adds a new GraphControl to the DiagramViewModel.
		/// </summary>
		public void AddGraphControl()
		{
			if (ChosenVideo == null)
				return;
			var graph = new DiagramGraph { Video = NodeModel.Inputs[Videos.IndexOf(ChosenVideo)] };
			var graphControl = new GraphControl { Parent = this, Video = ChosenVideo, Types = new ObservableCollection<Tuple<string, IGraphType>>(Types), DisplayTypes = new ObservableCollection<Tuple<string, IGraphType>>(Types), Graph = graph };
			graphControl.SetDisplayTypes();
			if (Reference.Item2 != null) {
				graphControl.ReferenceSet = true;
				graphControl.ReferenceHasLogfile = Reference.Item2.Source.Node.OutputHasLogfile;
			}
			GraphControls.Add(graphControl);
		}

		/// <summary>
		/// Converts a IEnumberable of KeyValuePairs to an ObservableDataSource. The KeyValuePair is converted to a Point.
		/// </summary>
		private static ObservableDataSource<Point> ConvertToDataSource(IEnumerable<KeyValuePair<int, double>> data)
		{
			var inData = new ObservableCollection<Point>(data.Select(datum => new Point(datum.Key, datum.Value)));
			var outData = new ObservableDataSource<Point>(inData);
			outData.SetXMapping(k => k.X);
			outData.SetYMapping(k => k.Y);
			return outData;
		}
	}
}