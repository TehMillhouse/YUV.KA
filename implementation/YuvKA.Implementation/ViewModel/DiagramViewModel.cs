using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class DiagramViewModel : OutputWindowViewModel
	{
		private ObservableCollection<LineGraphViewModel> lineGraphs;
		private Grid vidGrid = new Grid();
		public DiagramViewModel(Node nodeModel)
			: base(nodeModel)
		{
		}

		public Grid VidGrid { get { return vidGrid; } private set { vidGrid = value; } }


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



		public void DeleteGraph(DiagramGraph graph)
		{
			NodeModel.Graphs.Remove(graph);
		}

		public void AddGraph(Node.Input video)
		{
			var newGraph = new DiagramGraph { Video = video };
			NodeModel.Graphs.Add(newGraph);
		}

		private void AddVideoToGrid()
		{
			var newB = new Button { Content = "Delete", Command = Delete() };
			VidGrid.Children.Add(newB);

			var newL = new Label { Content = ChosenVideo.Item1 };
			VidGrid.Children.Add(newL);

			var newC = new ComboBox { ItemsSource = Types };
			var newCT = new Tuple<string, IGraphType>(null, null);
			newC.SelectedItem = newCT;
		}
	}
}