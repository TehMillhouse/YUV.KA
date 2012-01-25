using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class InOutputViewModel : ViewAware
	{
		public InOutputViewModel(object model, NodeViewModel parent)
		{
			Model = model;
			Parent = parent;
		}

		public object Model { get; private set; }
		public NodeViewModel Parent { get; private set; }
		public bool IsFake { get { return Model == null; } }

		/// <summary>
		/// Returns the view's current midpoint immediately on subscription, whenever
		/// the position of the parent's view changes and when this view model's view has loaded
		/// </summary>
		public IObservable<Point> Midpoint
		{
			get
			{
				var getPos = IoC.Get<IGetPosition>();

				// Whenever one of the sources mentioned above triggers...
				return Observable.Merge(
					Observable.Return(Unit.Default),
					Parent.ViewPositionChanged,
					IoC.Get<IGetPosition>().ViewLoaded(this)
				)
					// ...compute the new midpoint
				.Select(_ => {
					Point? pos = getPos.GetElementPosition(this, relativeTo: Parent.Parent);
					if (pos != null) {
						double width = getPos.GetElementSize(this).Width;
						Vector mid = new Vector(width, width) / 2;
						pos = pos.Value + mid;
					}
					return pos;
				}).Where(pos => pos.HasValue).Select(pos => pos.Value); // ignore events before view creation
			}
		}
	}
}