using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	/// <summary>
	/// Prvides the output window of a HistogramNode
	/// </summary>
	public class HistogramViewModel : OutputWindowViewModel, INotifyPropertyChanged
	{
		/// <summary>
		/// Creates a new HistogramViewMode with the given HistogramNode.
		/// </summary>
		public HistogramViewModel(HistogramNode nodeModel)
			: base(nodeModel, null)
		{
		}

		/// <summary>
		/// Gets the HistogramNode of this output window.
		/// </summary>
		public new HistogramNode NodeModel { get { return (HistogramNode)base.NodeModel; } }

		/// <summary>
		/// Get or sets the Data of the HistogramNode
		/// </summary>
		public EnumerableDataSource<KeyValuePair<int, double>> Data { get; private set; }

		/// <summary>
		/// Updates the graph of the histogram with the newly calculated data of the frame
		/// once it is rendered.
		/// </summary>
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
