using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Models an edge connecting two nodes in the view. In the pipeline model, edges are not
	/// modelled explictly but implied by the Node.Input.Source property.
	/// </summary>
	public class EdgeViewModel : PropertyChangedBase, IDisposable
	{
		Point startPoint, endPoint;
		InOutputViewModel startVM, endVM;
		// objects for disposing the subscriptions on InOutputVM.Midpoint
		IDisposable startHandler, endHandler;
		EdgeStatus status;

		public EdgeStatus Status
		{
			get { return status == EdgeStatus.Indeterminate && Parent.Fabulous ? EdgeStatus.Fabulous : status; }
			set
			{
				status = value;
				NotifyOfPropertyChange(() => Status);
			}
		}

		public PipelineViewModel Parent { get; set; }

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

		/// <summary>
		/// Gets or sets the start in-/output connected to this edge.
		/// If set, the StartPoint property will be held in sync to the in-/output.
		/// </summary>
		public InOutputViewModel StartViewModel
		{
			get { return startVM; }
			set
			{
				startVM = value;
				startHandler = value.Midpoint.Subscribe(pos => StartPoint = pos);
			}
		}

		/// <summary>
		/// Gets or sets the end in-/output connected to this edge.
		/// If set, the EndPoint property will be held in sync to the in-/output.
		/// </summary>
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

		/// <summary>
		/// Gets a Geometry object visualizing the edge.
		/// </summary>
		public Geometry Geometry
		{
			get
			{
				Vector delta = EndPoint - StartPoint;
				Vector horizDelta = new Vector(Math.Abs(delta.X), 0);
				if (!(StartViewModel.Model is Node.Output))
					horizDelta *= -1;

				var geo = new StreamGeometry();
				using (var ctx = geo.Open()) {
					ctx.BeginFigure(StartPoint, isFilled: false, isClosed: false);
					ctx.BezierTo(StartPoint + horizDelta / 2, EndPoint - horizDelta / 2, EndPoint, isStroked: true, isSmoothJoin: false);
				}
				return geo;
			}
		}

		/// <summary>
		/// Try and sort two InOutputViewModels into input and output.
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

		/// <summary>
		/// Removes all handlers notifying of position changes of the nodes this edge is connected to.
		/// </summary>
		public void Dispose()
		{
			if (startHandler != null)
				startHandler.Dispose();
			if (endHandler != null)
				endHandler.Dispose();
		}
	}
}
