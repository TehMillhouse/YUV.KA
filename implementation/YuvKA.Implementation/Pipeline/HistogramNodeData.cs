using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace YuvKA.Pipeline.Implementation
{
	class HistogramNodeData : ObservableCollection<KeyValuePair<int, double>>
	{
		private static HistogramNodeData histogramNodeData;
		public HistogramNodeData(double[] data)
		{
			histogramNodeData = this;
			init(data);
		}

		public static HistogramNodeData getModel()
		{
			return histogramNodeData;
		}

		public void init(double[] data)
		{
			int i = 0;
			foreach (double d in data) {
				Add(new KeyValuePair<int, double>(i, d));
				i++;
			}
		}
		public ObservableCollection<KeyValuePair<int, double>> getData()
		{
			return this;
		}
	}
}
