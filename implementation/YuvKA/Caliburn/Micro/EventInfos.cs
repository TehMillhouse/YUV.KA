using System.Windows;
using System.Windows.Input;

namespace Caliburn.Micro
{
	public interface IDragEventInfo
	{
		T GetData<T>();
		Point GetPosition(IViewAware relativeTo);
		DragDropEffects Effects { get; set; }
	}

	public class WpfDragEventInfo : IDragEventInfo
	{
		DragEventArgs e;

		public WpfDragEventInfo(DragEventArgs e)
		{
			this.e = e;
		}

		public T GetData<T>()
		{
			return (T)e.Data.GetData(typeof(T));
		}

		public Point GetPosition(IViewAware relativeTo)
		{
			return e.GetPosition((IInputElement)relativeTo.GetView());
		}

		public DragDropEffects Effects { get { return e.Effects; } set { e.Effects = value; } }
	}


	public interface IMouseEventInfo
	{
		Point GetPosition(IViewAware relativeTo);
		MouseButtonState LeftButton { get; }
	}

	public class WpfMouseEventInfo : IMouseEventInfo
	{
		MouseEventArgs e;

		public WpfMouseEventInfo(MouseEventArgs e)
		{
			this.e = e;
		}

		public Point GetPosition(IViewAware relativeTo)
		{
			return e.GetPosition((IInputElement)relativeTo.GetView());
		}

		public MouseButtonState LeftButton { get { return e.LeftButton; } }
	}
}
