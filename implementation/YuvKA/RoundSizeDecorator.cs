using System;
using System.Windows;
using System.Windows.Controls;

namespace YuvKA
{
	/// <summary>
	/// Works around weird WPF bug.
	/// </summary>
	public class RoundSizeDecorator : Decorator
	{
		protected override Size MeasureOverride(Size constraint)
		{
			Size size = base.MeasureOverride(constraint);
			return new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			Size size = base.ArrangeOverride(arrangeSize);
			return new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
		}
	}
}
