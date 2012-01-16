using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class EdgeViewModel : PropertyChangedBase
	{
		InOutputViewModel input, output;
		PipelineViewModel parent;

		public EdgeViewModel(PipelineViewModel parent, InOutputViewModel input, InOutputViewModel output)
		{
			this.parent = parent;
			this.input = input;
			this.output = output;
			input.ViewAttached += delegate { NotifyOfPropertyChange(() => Geometry); };
			output.ViewAttached += delegate { NotifyOfPropertyChange(() => Geometry); };
		}

		public Geometry Geometry
		{
			get
			{
				if (input.GetView() == null || output.GetView() == null)
					return null;

				var getPos = IoC.Get<IGetPosition>();
				double inputWidth = getPos.GetElementSize(input).Width;
				Vector mid = new Vector(inputWidth, inputWidth) / 2;
				Point startPoint = getPos.GetElementPosition(input, relativeTo: parent) + mid;
				Point endPoint = getPos.GetElementPosition(output, relativeTo: parent) + mid;
				Vector delta = endPoint - startPoint;
				Vector horizDelta = new Vector(delta.X, 0);

				var geo = new StreamGeometry();
				using (var ctx = geo.Open()) {
					ctx.BeginFigure(startPoint, isFilled: false, isClosed: false);
					ctx.BezierTo(startPoint + horizDelta / 2, endPoint - horizDelta / 2, endPoint, isStroked: true, isSmoothJoin: false);
				}
				return geo;
			}
		}
	}
}
