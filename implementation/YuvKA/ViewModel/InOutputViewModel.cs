using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	public class InOutputViewModel : ViewAware
	{
		// Prependes the given observable with an immediate OnNext notification on subscrition
		static IObservable<Unit> Immediate(IObservable<Unit> obs)
		{
			return Observable.StartWith(obs, Unit.Default);
		}

		public InOutputViewModel(object model, NodeViewModel parent)
		{
			Model = model;
			Parent = parent;
		}

		public object Model { get; private set; }
		public NodeViewModel Parent { get; private set; }
		public bool IsFake { get { return Model == null; } }

		/// <summary>
		/// Returns the view's current midpoint immediately on subscription and whenever
		/// the position of the parent's view changes
		/// </summary>
		public IObservable<Point> Midpoint
		{
			get
			{
				var getPos = IoC.Get<IGetPosition>();

				return Immediate(Parent.ViewPositionChanged).Select(_ => {
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