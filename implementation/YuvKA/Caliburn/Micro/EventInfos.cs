using System.Windows;
using System.Windows.Input;

namespace Caliburn.Micro
{
	public interface IDragEventInfo
	{
		DragDropEffects Effects { get; set; }
		T GetData<T>();
		Point GetPosition(IViewAware relativeTo);
	}

	public interface IMouseEventInfo
	{
		MouseButtonState LeftButton { get; }
		Point GetPosition(IViewAware relativeTo);
	}


	public class WpfDragEventInfo : IDragEventInfo
	{
		DragEventArgs e;

		public WpfDragEventInfo(DragEventArgs e)
		{
			this.e = e;
		}

		public DragDropEffects Effects { get { return e.Effects; } set { e.Effects = value; } }

		public T GetData<T>()
		{
			return (T)e.Data.GetData(typeof(T));
		}

		public Point GetPosition(IViewAware relativeTo)
		{
			return e.GetPosition((IInputElement)relativeTo.GetView());
		}
	}

	public class WpfMouseEventInfo : IMouseEventInfo
	{
		MouseEventArgs e;

		public WpfMouseEventInfo(MouseEventArgs e)
		{
			this.e = e;
		}

		public MouseButtonState LeftButton { get { return e.LeftButton; } }

		public Point GetPosition(IViewAware relativeTo)
		{
			return e.GetPosition((IInputElement)relativeTo.GetView());
		}
	}
}
