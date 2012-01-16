using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YuvKA.Pipeline.Implementation
{
	class HistogramNodeData : ObservableCollection<KeyValuePair<int, double>>
	{
		private static HistogramNodeData histogramNodeData;
		public HistogramNodeData(double[] data)
		{
			histogramNodeData = this;
			Init(data);
		}

		public static HistogramNodeData GetModel()
		{
			return histogramNodeData;
		}

		public void Init(double[] data)
		{
			int i = 0;
			foreach (double d in data) {
				Add(new KeyValuePair<int, double>(i, d));
				i++;
			}
		}
		public ObservableCollection<KeyValuePair<int, double>> GetData()
		{
			return this;
		}
	}
}
