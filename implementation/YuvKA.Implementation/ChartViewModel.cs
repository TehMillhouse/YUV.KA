using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Media;

namespace YuvKA.Implementation
{
    public class ChartViewModel:ViewModelBase
    {
        private ObservableCollection<LineGraphViewModel> lineGraphs;
        private ICommand addLinesCommand;
        private ICommand editLineCommand;
        private HorizontalDateTimeAxis dateAxis;
        private CompositeDataSource editedDs;
        private bool toggle=true;

        public ChartViewModel()
        {
			this.lineGraphs = new ObservableCollection<LineGraphViewModel>();
        	lineGraphs.Add(new LineGraphViewModel
        	               	{
        	               		LineAndMarker = true,
        	               		PointDataSource = editedDs,
        	               		Name = "Test1",
        	               		Thickness = 2,
        	               		Color = Color.FromRgb(0, 0, 255)
        	               	});
        }

        public ObservableCollection<LineGraphViewModel> LineGraphs
        {
            get
            {
                if (this.lineGraphs == null)
                {
                    this.lineGraphs = new ObservableCollection<LineGraphViewModel>();
                }

                return this.lineGraphs;
            }
        }

        private void EditLineGraph()
        {
            LineGraphViewModel lineGraphViewModel = new LineGraphViewModel();
            if (this.toggle)
            {
                lineGraphViewModel.LineAndMarker = true;
                lineGraphViewModel.PointDataSource = editedDs;
                lineGraphViewModel.Name = "Test1";
                lineGraphViewModel.Thickness = 2;
                lineGraphViewModel.Color = Color.FromRgb(0, 0, 255);
                this.LineGraphs[0] = lineGraphViewModel;
                this.toggle = false;
            }
            else
            {
                lineGraphViewModel.LineAndMarker = false;
                lineGraphViewModel.PointDataSource = editedDs;
                lineGraphViewModel.Name = "Test1";
                lineGraphViewModel.Color = Color.FromRgb(255, 0, 0);
                lineGraphViewModel.Thickness = 4;
                this.LineGraphs[0] = lineGraphViewModel;
                this.toggle = true;
            }
        }

        private void AddLineGraphs()
        {
			const int N = 10;
			const int M = 20;
			double[] x = new double[N];
			double[] y = new double[N];

			////double[] x1 = new double[N];
			////double[] y1 = new double[N];
			double[] x1 = new double[M];
			double[] y1 = new double[M];
			DateTime[] date1 = new DateTime[M];

			DateTime[] date = new DateTime[N];

			for (int i = 0; i < N * 2; i = i + 2) {
				x[i / 2] = i / 2 * 0.1;
				//x1[i/2] = i/2 * 0.2;
				y[i / 2] = Math.Sin(x[i / 2]);
				//y1[i/2] = Math.Cos(x1[i/2]) * this.factor;
				date[i / 2] = DateTime.Now.AddMinutes(-(N * 2) + i / 2);
			}

			for (int i = 0; i < M; i++) {

				x1[i] = i * 0.2;
				y1[i] = Math.Cos(x1[i]) * 2;
				date1[i] = DateTime.Now.AddMinutes(-M + i);
			}

			EnumerableDataSource<double> xs = new EnumerableDataSource<double>(y);
			xs.SetYMapping(_y => _y);
			EnumerableDataSource<DateTime> ys = new EnumerableDataSource<DateTime>(date);
			this.dateAxis = new HorizontalDateTimeAxis();
			ys.SetXMapping(dateAxis.ConvertToDouble);
			CompositeDataSource ds = new CompositeDataSource(xs, ys);
			LineGraphViewModel lineGraphViewModel = new LineGraphViewModel();
			lineGraphViewModel.PointDataSource = ds;
			this.editedDs = ds;
			lineGraphViewModel.Name = "Test1";
			lineGraphViewModel.Color = Color.FromRgb(255, 0, 0);
			lineGraphViewModel.EntityId = Guid.NewGuid();
			lineGraphViewModel.LineAndMarker = false;
			lineGraphViewModel.Thickness = 1;
			this.LineGraphs.Add(lineGraphViewModel);

			EnumerableDataSource<double> xs1 = new EnumerableDataSource<double>(y1);
			xs1.SetYMapping(_y1 => _y1 / 2);
			EnumerableDataSource<DateTime> ys1 = new EnumerableDataSource<DateTime>(date1);
			ys1.SetXMapping(dateAxis.ConvertToDouble);
			CompositeDataSource ds1 = new CompositeDataSource(xs1, ys1);

			lineGraphViewModel = new LineGraphViewModel();
			lineGraphViewModel.PointDataSource = ds1;
			lineGraphViewModel.Name = "Test2";
			lineGraphViewModel.Color = Color.FromRgb(0, 0, 255);
			lineGraphViewModel.EntityId = Guid.NewGuid();
			lineGraphViewModel.LineAndMarker = true;
			lineGraphViewModel.Thickness = 1;
			this.LineGraphs.Add(lineGraphViewModel);
        }
    }
}
