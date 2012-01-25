using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class DiagramViewModel : OutputWindowViewModel, IHandle<DeleteGraphControlMessage>
	{
		private ObservableCollection<LineGraphViewModel> lineGraphs;

		private ICommand addLinesCommand;

		public ICommand AddLinesCommand
		{
			get
			{
				if (this.addLinesCommand == null) {
					this.addLinesCommand = new RelayCommand(param => this.AddLineGraphs(), param => true);
				}

				return this.addLinesCommand;
			}
		}

		private void AddLineGraphs()
		{
			LineGraphViewModel line1 = new LineGraphViewModel();
			ObservableCollection<KeyValuePair<int, double>> inDat = new ObservableCollection<KeyValuePair<int, double>>();
			inDat.Add(new KeyValuePair<int, double>(0, 10.5));
			inDat.Add(new KeyValuePair<int, double>(1, 40.6));

			EnumerableDataSource<KeyValuePair<int, double>> data = new EnumerableDataSource<KeyValuePair<int, double>>(inDat);
			data.SetXMapping(k => k.Key);
			data.SetYMapping(k => k.Value);
			CompositeDataSource ds = new CompositeDataSource(data);
			line1.PointDataSource = ds;
			line1.Name = "Test1";
			line1.Color = Color.FromRgb(255, 0, 0);
			line1.EntityId = Guid.NewGuid();
			line1.LineAndMarker = false;
			line1.Thickness = 1;

			LineGraphViewModel line2 = new LineGraphViewModel();
			ObservableCollection<KeyValuePair<int, double>> inDat2 = new ObservableCollection<KeyValuePair<int, double>>();
			inDat2.Add(new KeyValuePair<int, double>(0, 250));
			inDat2.Add(new KeyValuePair<int, double>(1, 200));

			EnumerableDataSource<KeyValuePair<int, double>> data2 = new EnumerableDataSource<KeyValuePair<int, double>>(inDat2);
			data2.SetXMapping(k => k.Key);
			data2.SetYMapping(k => k.Value);
			CompositeDataSource ds2 = new CompositeDataSource(data2);
			line2.PointDataSource = ds2;
			line2.Name = "Test2";
			line2.Color = Color.FromRgb(0, 0, 255);
			line2.EntityId = Guid.NewGuid();
			line2.LineAndMarker = false;
			line2.Thickness = 1;
			this.LineGraphs.Add(line1);
			this.LineGraphs.Add(line2);
			//const int N = 10;
			//const int M = 20;
			//double[] x = new double[N];
			//double[] y = new double[N];

			//////double[] x1 = new double[N];
			//////double[] y1 = new double[N];
			//double[] x1 = new double[M];
			//double[] y1 = new double[M];
			//DateTime[] date1 = new DateTime[M];

			//DateTime[] date = new DateTime[N];

			//for (int i = 0; i < N * 2; i = i + 2)
			//{
			//    x[i / 2] = i / 2 * 0.1;
			//    //x1[i/2] = i/2 * 0.2;
			//    y[i / 2] = Math.Sin(x[i / 2]);
			//    //y1[i/2] = Math.Cos(x1[i/2]) * this.factor;
			//    date[i / 2] = DateTime.Now.AddMinutes(-(N * 2) + i / 2);
			//}

			//for (int i = 0; i < M; i++)
			//{

			//    x1[i] = i * 0.2;
			//    y1[i] = Math.Cos(x1[i]) * 2;
			//    date1[i] = DateTime.Now.AddMinutes(-M + i);
			//}

			//EnumerableDataSource<double> xs = new EnumerableDataSource<double>(y);

			//xs.SetYMapping(_y => _y);
			//EnumerableDataSource<DateTime> ys = new EnumerableDataSource<DateTime>(date);
			//this.dateAxis = new HorizontalDateTimeAxis();
			//ys.SetXMapping(dateAxis.ConvertToDouble);
			//CompositeDataSource ds = new CompositeDataSource(xs, ys);
			//LineGraphViewModel lineGraphViewModel = new LineGraphViewModel();
			//lineGraphViewModel.PointDataSource = ds;
			//this.editedDs = ds;
			//lineGraphViewModel.Name = "Test1";
			//lineGraphViewModel.Color = Color.FromRgb(255, 0, 0);
			//lineGraphViewModel.EntityId = Guid.NewGuid();
			//lineGraphViewModel.LineAndMarker = false;
			//lineGraphViewModel.Thickness = 1;
			//this.LineGraphs.Add(lineGraphViewModel);

			//EnumerableDataSource<double> xs1 = new EnumerableDataSource<double>(y1);
			//xs1.SetYMapping(_y1 => _y1 / 2);
			//EnumerableDataSource<DateTime> ys1 = new EnumerableDataSource<DateTime>(date1);
			//ys1.SetXMapping(dateAxis.ConvertToDouble);
			//CompositeDataSource ds1 = new CompositeDataSource(xs1, ys1);

			//lineGraphViewModel = new LineGraphViewModel();
			//lineGraphViewModel.PointDataSource = ds1;
			//lineGraphViewModel.Name = "Test2";
			//lineGraphViewModel.Color = Color.FromRgb(0, 0, 255);
			//lineGraphViewModel.EntityId = Guid.NewGuid();
			//lineGraphViewModel.LineAndMarker = true;
			//lineGraphViewModel.Thickness = 1;
			//this.LineGraphs.Add(lineGraphViewModel);
		}

		public DiagramViewModel(Node nodeModel)
			: base(nodeModel)
		{
		}

		private ICommand add;

		public ICommand Add
		{
			get
			{
				if (add == null)
					add = new RelayCommand(param => AddGraphControl(), param => true);
				return add;
			}
		}

		private void AddGraphControl()
		{
			var graph = new DiagramGraph { Video = NodeModel.Inputs[Videos.IndexOf(ChosenVideo)] };
			var graphControl = new GraphControl { Video = ChosenVideo, Types = Types, Graph = graph };
			GraphControls.Add(graphControl);
			AddGraph(graph);

			var line1 = new LineGraphViewModel();
			var inDat1 = new ObservableCollection<KeyValuePair<int, double>>();
			inDat1.Add(new KeyValuePair<int, double>(0, 10.5));
			inDat1.Add(new KeyValuePair<int, double>(1, 40.6));

			var data1 = new EnumerableDataSource<KeyValuePair<int, double>>(inDat1);
			data1.SetXMapping(k => k.Key);
			data1.SetYMapping(k => k.Value);
			var ds1 = new CompositeDataSource(data1);
			line1.PointDataSource = ds1;
			line1.Name = "Test1";
			line1.Color = Color.FromRgb(255, 0, 0);
			line1.EntityId = Guid.NewGuid();
			line1.LineAndMarker = false;
			line1.Thickness = 1;

			//LineGraphs.Add(line1);

			AddLineGraphViewModel(graphControl);
		}

		public int del()
		{

			return 0;
		}

		public ObservableCollection<LineGraphViewModel> LineGraphs
		{
			get
			{
				if (this.lineGraphs == null) {
					this.lineGraphs = new ObservableCollection<LineGraphViewModel>();
				}

				return this.lineGraphs;
			}
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
			var inDat = new ObservableCollection<KeyValuePair<int, double>>();


			var index = 0;
			foreach (var d in graphControl.Graph.Data) {
				inDat.Add(new KeyValuePair<int, double>(index, d));
				index++;
			}

			var data = new ObservableDataSource<KeyValuePair<int, double>>(inDat);

			data.SetXMapping(k => k.Key);
			data.SetYMapping(k => k.Value);
			var ds = new CompositeDataSource(data);

			line.PointDataSource = ds;
			if (graphControl.ChosenType != null) {
				line.Name = graphControl.Video.Item1 + " - " + graphControl.ChosenType.Item1;
			}
			else {
				line.Name = graphControl.Video.Item1;
			}
			line.Color = graphControl.LineColor;
			line.EntityId = Guid.NewGuid();
			line.LineAndMarker = false;
			line.Thickness = 1;

			LineGraphs.Add(line);

			//var line1 = new LineGraphViewModel();
			//var inDat1 = new ObservableCollection<KeyValuePair<int, double>>();
			//inDat.Add(new KeyValuePair<int, double>(0, 10.5));
			//inDat.Add(new KeyValuePair<int, double>(1, 40.6));

			//var data1 = new EnumerableDataSource<KeyValuePair<int, double>>(inDat1);
			//data.SetXMapping(k => k.Key);
			//data.SetYMapping(k => k.Value);
			//var ds1 = new CompositeDataSource(data1);
			//line1.PointDataSource = ds1;
			//line1.Name = "Test1";
			//line1.Color = Color.FromRgb(255, 0, 0);
			//line1.EntityId = Guid.NewGuid();
			//line1.LineAndMarker = false;
			//line1.Thickness = 1;

			//LineGraphs.Add(line1);
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
			GraphControls.Remove(message.GraphViewtoDelete);
			DeleteGraph(message.GraphViewtoDelete.Graph);
		}

		private ObservableDataSource<KeyValuePair<int, double>> ConvertToDataSource(IList<double> data)
		{
			var inData = new ObservableCollection<KeyValuePair<int, double>>();
			foreach (var d in data) {
				inData.Add(new KeyValuePair<int, double>(data.IndexOf(d), d));
			}
			var outData = new ObservableDataSource<KeyValuePair<int, double>>(inData);
			outData.SetXMapping(k => k.Key);
			outData.SetYMapping(k => k.Value);
			return outData;
		}

		public override void Handle(Pipeline.TickRenderedMessage message)
		{
			foreach (var l in LineGraphs) {
				l.PointDataSource = new CompositeDataSource(ConvertToDataSource(NodeModel.Graphs[LineGraphs.IndexOf(l)].Data));
				NotifyOfPropertyChange(() => l.PointDataSource);
			}

		}
	}
}