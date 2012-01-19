using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Pipeline.Implementation;
using YuvKA.Pipeline;
using System.Linq;

namespace YuvKA.ViewModel.Implementation
{
	public class HistogramViewModel : OutputWindowViewModel, INotifyPropertyChanged
	{
		public HistogramViewModel(HistogramNode nodeModel)
			: base(nodeModel)
		{
		}

		public new HistogramNode NodeModel { get { return (HistogramNode)base.NodeModel; } }
		private EnumerableDataSource<KeyValuePair<int, double>> data;

		public EnumerableDataSource<KeyValuePair<int, double>> Data
		{
			get
			{
				return data;
			}
			set
			{
				value.SetXMapping(k => k.Key);
				value.SetYMapping(k => k.Value);
				data = value;
			}
		}

		public EnumerableDataSource<KeyValuePair<int, double>> setData(double[] data)
		{
			ObservableCollection<KeyValuePair<int, double>> inData = new ObservableCollection<KeyValuePair<int, double>>();
			int i = 0;
			foreach (double d in data) {
				inData.Add(new KeyValuePair<int, double>(i, d));
				i++;
			}

			EnumerableDataSource<KeyValuePair<int, double>> outData = new EnumerableDataSource<KeyValuePair<int, double>>(inData);
			outData.SetXMapping(k => k.Key);
			outData.SetYMapping(k => k.Value);
			return outData;
		}

		public override void Handle(Pipeline.TickRenderedMessage message)
		{
			Data = new EnumerableDataSource<KeyValuePair<int, double>>(NodeModel.Data.Select((datum, idx) => new KeyValuePair<int, double>(idx, datum)));
			NotifyOfPropertyChange(() => Data);
		}
	}
}
