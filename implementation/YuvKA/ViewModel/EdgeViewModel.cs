using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class EdgeViewModel : PropertyChangedBase
	{
		Point startPoint, endPoint;
		InOutputViewModel startVM, endVM;
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
				var getPos = IoC.Get<IGetPosition>();
				DoWhenViewAvailable(value, () => {
					double width = getPos.GetElementSize(value).Width;
					Vector mid = new Vector(width, width) / 2;
					StartPoint = getPos.GetElementPosition(value, relativeTo: parent) + mid;
				});
			}
		}

		public InOutputViewModel EndViewModel
		{
			get { return endVM; }
			set
			{
				endVM = value;
				var getPos = IoC.Get<IGetPosition>();
				DoWhenViewAvailable(value, () => {
					double width = getPos.GetElementSize(value).Width;
					Vector mid = new Vector(width, width) / 2;
					EndPoint = getPos.GetElementPosition(value, relativeTo: parent) + mid;
				});
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

		void DoWhenViewAvailable(IViewAware element, System.Action action)
		{
			if (element.GetView() != null)
				action();
			else {
				EventHandler<ViewAttachedEventArgs> handler = null;
				handler = delegate
				{
					action();
					element.ViewAttached -= handler;
				};
				element.ViewAttached += handler;
			}
		}
	}
}
