using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Common view model of both Node.Input and Node.Output
	/// </summary>
	public class InOutputViewModel : ViewAware
	{
		/// <summary>
		/// Initializes an InOutputViewModel instance with a model of type Node.Input or Node.Output
		/// and the model's node's view model.
		/// </summary>
		public InOutputViewModel(object model, NodeViewModel parent)
		{
			Model = model;
			Parent = parent;
		}

		/// <summary>
		/// Gets the represented model of this view model, either a Node.Input or a Node.Output
		/// </summary>
		public object Model { get; private set; }

		public NodeViewModel Parent { get; private set; }

		/// <summary>
		/// A fake is used to represent inputs that aren't added to the model yet, i.e. for
		/// nodes with a variable number of inputs.
		/// </summary>
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
					IoC.Get<IGetPosition>().ViewLoaded(this),
					IoC.Get<IGetPosition>().ViewLoaded(Parent)
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