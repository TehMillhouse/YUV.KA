using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using YuvKA.Pipeline.Implementation;
using YuvKA.Pipeline;
using System.Linq;

namespace YuvKA.ViewModel.Implementation
{
	public class HistogramViewModel : OutputWindowViewModel
	{
		public HistogramViewModel(HistogramNode nodeModel)
			: base(nodeModel) { }

		public new HistogramNode NodeModel { get { return (HistogramNode)base.NodeModel; } }
		public IEnumerable<KeyValuePair<int, double>> Data { get; private set; }

		public override void Handle(Pipeline.TickRenderedMessage message)
		{
			Data = NodeModel.Data.Select((datum, idx) => new KeyValuePair<int, double>(idx, datum));
			NotifyOfPropertyChange(() => Data);
		}
	}
}
