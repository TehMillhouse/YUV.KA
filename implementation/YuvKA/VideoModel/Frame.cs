using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.VideoModel
{
	public class Frame
	{
		public Size Size { get; }

		public Frame(Size size)
		{
			Data = new Rgb[size.Height * size.Width];
			Size = size;
		}

		public Rgb this[int x, int y]
		{
			get { return Data[y * Size.Width + x]; }
			set { Data[y * Size.Width + x] = value; }
		}
	}
}
