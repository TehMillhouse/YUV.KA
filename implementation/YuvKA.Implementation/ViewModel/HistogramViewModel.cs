using System.Collections.Generic;
using System.Collections.ObjectModel;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class HistogramViewModel : OutputWindowViewModel
	{

		private HistogramNodeData model;
		public HistogramViewModel()
		{
			Node = new HistogramNode();
			model = new HistogramNodeData(Node.Data);
			data = model;
		}

		public HistogramNode Node { get; set; }

		private ObservableCollection<KeyValuePair<int, double>> data;

		public ObservableCollection<KeyValuePair<int, double>> Data
		{
			get { return data; }
		}

		public new HistogramNode NodeModel { get; set; }

		public override void Handle(Pipeline.TickRenderedMessage message)
		{

		}
	}
}
