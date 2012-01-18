using System.Collections.Generic;
using System.Collections.ObjectModel;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class DiagramViewModel : OutputWindowViewModel
	{
		public DiagramViewModel(Node nodeModel)
			: base(nodeModel)
		{
		}

		public new DiagramNode NodeModel { get; set; }


		public IList<IGraphType> Types
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public ObservableCollection<Node.Input> Videos
		{
			get
			{
				ObservableCollection<Node.Input> videos = new ObservableCollection<Node.Input>();
				foreach (Node.Input i in NodeModel.Inputs) {
					videos.Add(i);
				}
				return videos;
			}
		}

		public void DeleteGraph(DiagramGraph graph)
		{
			NodeModel.Graphs.Remove(graph);
		}

		public void AddGraph(Node.Input video)
		{
			DiagramGraph newGraph = new DiagramGraph();
			newGraph.Video = video;
			NodeModel.Graphs.Add(newGraph);
		}
	}
}