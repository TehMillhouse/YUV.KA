using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class HistogramViewModel : OutputWindowViewModel, INotifyPropertyChanged
	{
		public HistogramViewModel(HistogramNode nodeModel)
			: base(nodeModel)
		{
		}

		public new HistogramNode NodeModel { get { return (HistogramNode)base.NodeModel; } }
		public EnumerableDataSource<KeyValuePair<int, double>> Data { get; private set; }

		public override void Handle(Pipeline.TickRenderedMessage message)
		{
			Data = new EnumerableDataSource<KeyValuePair<int, double>>(
				NodeModel.Data.Select((datum, idx) => new KeyValuePair<int, double>(idx, datum))
			);
			Data.SetXMapping(k => k.Key);
			Data.SetYMapping(k => k.Value);
			NotifyOfPropertyChange(() => Data);
		}
	}
}
