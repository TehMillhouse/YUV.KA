using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace YuvKA.Implementation
{
	/// <summary>
	/// A class from the DynamicDataDisplay package, modified
	/// to be able to add and remove lines dynamically.
	/// </summary>
	public class ChartPlotter : Microsoft.Research.DynamicDataDisplay.ChartPlotter
	{
		/// <summary>
		/// DependencyProperty for LineGraphs
		/// </summary>
		public static readonly DependencyProperty LineGraphsProperty = DependencyProperty.Register("LineGraphs", typeof(ObservableCollection<LineGraphViewModel>), typeof(ChartPlotter), new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeLineGraphs)));

		private IDictionary<Guid, LineGraphViewModel> lineGraphsList;
		private IList<LineGraph> lineGraphLines = new List<LineGraph>();
		private IList<LineAndMarker<MarkerPointsGraph>> lineAndMarkerGraphs = new List<LineAndMarker<MarkerPointsGraph>>();

		/// <summary>
		/// Gets or sets the line graphs.
		/// </summary>
		/// <value>The line graphs.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "ok")]
		public ObservableCollection<LineGraphViewModel> LineGraphs
		{
			get
			{
				return (ObservableCollection<LineGraphViewModel>)GetValue(LineGraphsProperty);
			}

			set
			{
				SetValue(LineGraphsProperty, value);

				this.LineGraphs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnLineGraphsCollectionChanged);
			}
		}

		/// <summary>
		/// Changes the line graphs.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="eventArgs">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		public static void ChangeLineGraphs(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
		{
			(source as ChartPlotter).UpdateLineGraphs((ObservableCollection<LineGraphViewModel>)eventArgs.NewValue);
		}

		private void UpdateLineGraphs(ObservableCollection<LineGraphViewModel> lineGrphs)
		{
			this.LineGraphs = lineGrphs;
			this.lineGraphsList = new Dictionary<Guid, LineGraphViewModel>();
		}

		private void OnLineGraphsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add: {
						foreach (LineGraphViewModel viewModel in e.NewItems) {
							if (viewModel.LineAndMarker) {
								SolidColorBrush brush = new SolidColorBrush();
								brush.Color = viewModel.Color;
								LineAndMarker<MarkerPointsGraph> lineAndMarker = this.AddLineGraph(viewModel.PointDataSource, new Pen(brush, viewModel.Thickness), new CirclePointMarker { Size = 7, Fill = brush }, new PenDescription(viewModel.Name));
								lineAndMarker.LineGraph.Name = viewModel.Name;
								this.lineAndMarkerGraphs.Add(lineAndMarker);
							} else {
								LineGraph lineGraph = this.AddLineGraph(viewModel.PointDataSource, viewModel.Color, viewModel.Thickness, viewModel.Name);
								lineGraph.Name = viewModel.Name;
								this.lineGraphLines.Add(lineGraph);
							}
						}

						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Move: {
						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove: {
						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace: {
						bool tempBool = false;
						foreach (LineGraphViewModel viewModel in e.NewItems) {
							foreach (LineGraph line in this.lineGraphLines) {
								if (this.Children.Contains(line) && line.Name == viewModel.Name) {
									this.Children.Remove(line);
									this.lineGraphLines.Remove(line);
									tempBool = true;
									break;
								}
							}

							foreach (LineAndMarker<MarkerPointsGraph> line in this.lineAndMarkerGraphs) {
								if (this.Children.Contains(line.LineGraph) && line.LineGraph.Name == viewModel.Name) {
									this.Children.Remove(line.LineGraph);
									this.Children.Remove(line.MarkerGraph);
									this.lineAndMarkerGraphs.Remove(line);
									tempBool = true;
									break;
								}
							}

							if (tempBool) {
								if (viewModel.LineAndMarker) {
									SolidColorBrush brush = new SolidColorBrush();
									brush.Color = viewModel.Color;
									LineAndMarker<MarkerPointsGraph> lineAndMarker = this.AddLineGraph(viewModel.PointDataSource, new Pen(brush, viewModel.Thickness), new CirclePointMarker { Size = 7, Fill = brush }, new PenDescription(viewModel.Name));
									lineAndMarker.LineGraph.Name = viewModel.Name;
									this.lineAndMarkerGraphs.Add(lineAndMarker);
								} else {
									LineGraph lineGraph = this.AddLineGraph(viewModel.PointDataSource, viewModel.Color, viewModel.Thickness, viewModel.Name);
									lineGraph.Name = viewModel.Name;
									this.lineGraphLines.Add(lineGraph);
								}

								tempBool = false;
							}
						}

						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset: {
						break;
					}
			}
		}
	}
}
