using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class EdgeViewModel : PropertyChangedBase, IDisposable
	{
		Point startPoint, endPoint;
		InOutputViewModel startVM, endVM;
		// objects for disposing the subscriptions on InOutputVM.Midpoint
		IDisposable startHandler, endHandler;
		PipelineViewModel parent;
		EdgeStatus status;

		public EdgeViewModel(PipelineViewModel parent)
		{
			this.parent = parent;
		}

		public EdgeStatus Status
		{
			get { return status; }
			set
			{
				status = value;
				NotifyOfPropertyChange(() => Status);
			}
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
				if (endVM == value)
					return;
				endVM = value;
				if (endHandler != null)
					endHandler.Dispose();
				endHandler = value.Midpoint.Subscribe(pos => EndPoint = pos);
			}
		}

		public Geometry Geometry
		{
			get
			{
				var geo = new StreamGeometry();
				var delta = EndPoint - StartPoint;
				var angle = Math.Atan2(delta.Y, delta.X);
				var controlOffset = StartViewModel.Model is Node.Output ?
					new Vector(Math.Abs(angle) / Math.PI * 200, 0) :
					new Vector(-(1 - Math.Abs(angle) / Math.PI) * 200, 0);

				using (var ctx = geo.Open()) {
					ctx.BeginFigure(StartPoint, isFilled: false, isClosed: false);
					ctx.BezierTo(StartPoint + controlOffset, EndPoint - controlOffset, EndPoint, isStroked: true, isSmoothJoin: false);
				}
				return geo;
			}
		}

		/// <summary>
		/// Try and sort StartViewModel and EndViewModel into input and output.
		/// Returns false if there are two inputs or outputs.
		/// </summary>
		public bool GetInOut(out InOutputViewModel inputVM, out InOutputViewModel outputVM)
		{
			if ((StartViewModel.IsFake || StartViewModel.Model is Node.Input) && EndViewModel.Model is Node.Output) {
				inputVM = StartViewModel;
				outputVM = EndViewModel;
				return true;
			}
			if (StartViewModel.Model is Node.Output && (EndViewModel.IsFake || EndViewModel.Model is Node.Input)) {
				inputVM = EndViewModel;
				outputVM = StartViewModel;
				return true;
			}

			inputVM = outputVM = null;
			return false;
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
