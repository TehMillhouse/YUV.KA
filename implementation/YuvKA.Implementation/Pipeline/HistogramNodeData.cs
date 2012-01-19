using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YuvKA.Pipeline.Implementation
{
	class HistogramNodeData : ObservableCollection<KeyValuePair<int, double>>, INotifyPropertyChanged
	{

		public void setData(double[] data)
		{
			int i = 0;
			foreach (double d in data) {
				Add(new KeyValuePair<int, double>(i, d));
				i++;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(String info)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}
