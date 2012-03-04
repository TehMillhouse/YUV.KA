using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	/// <summary>
	/// Provides the output window of a DiagramNode
	/// </summary>
	public class DiagramViewModel : OutputWindowViewModel
	{
		private List<System.Windows.Media.Color> typeColors;
		Tuple<string, Node.Input> chosenVideo;

		/// <summary>
		/// Creates a new DiagramViewModel with the given DiagramNode.
		/// </summary>
		public DiagramViewModel(Node nodeModel)
			: base(nodeModel, null)
		{
			Types = IoC.GetAllInstances(typeof(IGraphType)).Select(o => new GraphTypeViewModel((IGraphType)o)).ToList();
			Graphs = new ObservableCollection<DiagramGraphViewModel>();
			NodeModel.Graphs.Select(g => new DiagramGraphViewModel(g, this)).Apply(Graphs.Add);

			NodeModel.Graphs.CollectionChanged += (_, e) => {
				if (e.Action == NotifyCollectionChangedAction.Remove) {
					foreach (DiagramGraph graph in e.OldItems)
						Graphs.Remove(Graphs.Single(g => g.Model == graph));
					NotifyOfPropertyChange(() => Lines);
				}
			};

			NodeModel.PropertyChanged += (_, e) => {
				if (e.PropertyName == "ReferenceVideo")
					NotifyOfPropertyChange(() => Reference);
			};
		}

		/// <summary>
		/// Gets the DiagramNode of this output window.
		/// </summary>
		public new DiagramNode NodeModel { get { return (DiagramNode)base.NodeModel; } }

		/// <summary>
		/// Gets all available IGraphTypes.
		/// </summary>
		public IList<GraphTypeViewModel> Types { get; private set; }

		/// <summary>
		/// Gets or sets the reference video of the DiagramNode.
		/// </summary>
		public Tuple<string, Node.Input> Reference
		{
			get { return NodeModel.ReferenceVideo != null ? new Tuple<string, Node.Input>(GetVideoName(NodeModel.ReferenceVideo), NodeModel.ReferenceVideo) : null; }
			set { NodeModel.ReferenceVideo = value.Item2; }
		}

		/// <summary>
		/// Gets the inputvideos of the DiagramNode and adds an index to them.
		/// </summary>
		public IEnumerable<Tuple<string, Node.Input>> Videos
		{
			get
			{
				return NodeModel.Inputs.Select(i => new Tuple<string, Node.Input>(GetVideoName(i), i));
			}
		}

		/// <summary>
		/// Gets or sets the video chosen by the user to be used for drawing a graph.
		/// </summary>
		public Tuple<string, Node.Input> ChosenVideo
		{
			get { return chosenVideo; }
			set
			{
				chosenVideo = value;
				NotifyOfPropertyChange(() => CanAddGraph);
			}
		}

		public ObservableCollection<DiagramGraphViewModel> Graphs { get; private set; }

		public IEnumerable<LineGraph> Lines
		{
			get { return Graphs.Select(g => g.Line); }
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
						} while (randomColors.FindIndex(color => IsInInterval(color.GetHue(), newColor.GetHue(), 25.0)) != -1 || newColor.GetHue().Equals(0.0) || newColor.GetBrightness().Equals(1.0) || newColor.GetBrightness().Equals(0.0));
						randomColors.Add(newColor);
						randomColorsMedia.Add(System.Windows.Media.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B));
					}
					typeColors = randomColorsMedia;
				}
				return typeColors;
			}
		}

		public bool CanAddGraph { get { return ChosenVideo != null; } }

		public void AddGraph()
		{
			var graph = new DiagramGraph { Video = ChosenVideo.Item2 };
			NodeModel.Graphs.Add(graph);
			Graphs.Add(new DiagramGraphViewModel(graph, this));
			NotifyOfPropertyChange(() => Lines);
		}

		public void DeleteGraph(DiagramGraphViewModel graph)
		{
			NodeModel.Graphs.Remove(graph.Model);
			Graphs.Remove(graph);
			NotifyOfPropertyChange(() => Lines);
		}

		/// <summary>
		/// Generates a new color of the same tone as the basecolor of the given type
		/// </summary>
		public System.Windows.Media.Color NewColor(IGraphType type)
		{
			System.Drawing.Color newColor;
			// Get basecolor of the type
			var baseColorWpf = TypeColors[Types.IndexOf(Types.Single(t => t.Model.GetType() == type.GetType()))];
			var baseColor = System.Drawing.Color.FromArgb(baseColorWpf.R, baseColorWpf.G, baseColorWpf.B);

			var h = baseColor.GetHue();

			var random = new Random();
			do {
				// Generate new color by using the hue of the basecolor and getting a random brightness and saturation.
				newColor = HslHelper.HslToRgb(h, random.NextDouble(), random.NextDouble());
			} while (!IsInInterval(0.5, newColor.GetSaturation(), 0.3) || !IsInInterval(0.5, newColor.GetBrightness(), 0.3) || Graphs.Any(g => SimilarColor(Color.FromArgb(g.LineColor.R, g.LineColor.G, g.LineColor.B), newColor)));

			return System.Windows.Media.Color.FromRgb(newColor.R, newColor.G, newColor.B);
		}

		public string GetVideoName(Node.Input input)
		{
			return "Input " + (NodeModel.Inputs.IndexOf(input) + 1) + " | " + (input.Source == null ? "" : input.Source.Node.Name);
		}

		/// <summary>
		/// Determines whether a number is inside a given interval
		/// </summary>
		static bool IsInInterval(double intervalCenter, double number, double intervalSize)
		{
			var difference = number - intervalCenter;
			return Math.Abs(difference).CompareTo(intervalSize) < 0;
		}

		static bool SimilarColor(Color c1, Color c2)
		{
			return IsInInterval(c1.GetBrightness(), c2.GetBrightness(), 0.12) && IsInInterval(c1.GetSaturation(), c2.GetSaturation(), 0.12);
		}
	}
}