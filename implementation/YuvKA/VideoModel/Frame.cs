using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.VideoModel
{
	public class Frame
	{
        public Size Size { get; private set; }

		public Rgb[] Data
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public Rgb this[int x, int y]
		{
			get { return Data[y * Size.Width + x]; }
			set { Data[y * Size.Width + x] = value; }
		}
	}
}
