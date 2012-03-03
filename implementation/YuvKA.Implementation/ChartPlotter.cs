using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay;

namespace YuvKA.Implementation
{
	/// <summary>
	/// A class from the DynamicDataDisplay package, modified
	/// to be able to add and remove lines dynamically.
	/// </summary>
	public class ChartPlotter : Microsoft.Research.DynamicDataDisplay.ChartPlotter
	{
		public static readonly DependencyProperty LineGraphsProperty = DependencyProperty.Register("LineGraphs", typeof(IEnumerable<LineGraph>), typeof(ChartPlotter), new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeLineGraphs)));

		List<LineGraph> lineGraphs = new List<LineGraph>();

		/// <summary>
		/// Gets or sets the line graphs.
		/// </summary>
		/// <value>The line graphs.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "ok")]
		public ObservableCollection<LineGraph> LineGraphs
		{
			get { return (ObservableCollection<LineGraph>)GetValue(LineGraphsProperty); }
			set { SetValue(LineGraphsProperty, value); }
		}

		static void ChangeLineGraphs(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
		{
			(source as ChartPlotter).UpdateLineGraphs((IEnumerable<LineGraph>)eventArgs.NewValue);
		}

		private void UpdateLineGraphs(IEnumerable<LineGraph> lineGraphs)
		{
			foreach (LineGraph graph in this.lineGraphs)
				Children.Remove(graph);
			this.lineGraphs = lineGraphs.ToList();
			this.lineGraphs.Apply(Children.Add);
		}
	}
}
