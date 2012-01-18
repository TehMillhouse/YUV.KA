using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using YuvKA.Pipeline.Implementation;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel.Implementation
{
	public class HistogramViewModel : OutputWindowViewModel
	{
		private HistogramNodeData model = new HistogramNodeData();

		public HistogramViewModel(Node nodeModel) : base(nodeModel)
		{
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
			model.Clear();
			model.setData(Node.Data);
			data = model;
		}
	}
}
