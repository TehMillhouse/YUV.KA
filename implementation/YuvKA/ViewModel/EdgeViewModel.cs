using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class EdgeViewModel : PropertyChangedBase, IDisposable
	{
		Point startPoint, endPoint;
		InOutputViewModel startVM, endVM;
		// objects for disposing the subscriptions on InOutputVM.Midpoint
		IDisposable startHandler, endHandler;
		PipelineViewModel parent;

		public EdgeViewModel(PipelineViewModel parent)
		{
			this.parent = parent;
		}

		public Point StartPoint
		{
			get { return startPoint; }
			set
			{
				startPoint = value;
				NotifyOfPropertyChange(() => Geometry);
			}
		}

		public Point EndPoint
		{
			get { return endPoint; }
			set
			{
				endPoint = value;
				NotifyOfPropertyChange(() => Geometry);
			}
		}

		public InOutputViewModel StartViewModel
		{
			get { return startVM; }
			set
			{
				startVM = value;
				startHandler = value.Midpoint.Subscribe(pos => StartPoint = pos);
			}
		}

		public InOutputViewModel EndViewModel
		{
			get { return endVM; }
			set
			{
				endVM = value;
				endHandler = value.Midpoint.Subscribe(pos => EndPoint = pos);
			}
		}

		public Geometry Geometry
		{
			get
			{
				Vector delta = EndPoint - StartPoint;
				Vector horizDelta = new Vector(delta.X, 0);

				var geo = new StreamGeometry();
				using (var ctx = geo.Open()) {
					ctx.BeginFigure(StartPoint, isFilled: false, isClosed: false);
					ctx.BezierTo(StartPoint + horizDelta / 2, EndPoint - horizDelta / 2, EndPoint, isStroked: true, isSmoothJoin: false);
				}
				return geo;
			}
		}

		public void Dispose()
		{
			if (startHandler != null)
				startHandler.Dispose();
			if (endHandler != null)
				endHandler.Dispose();
		}
	}
}
