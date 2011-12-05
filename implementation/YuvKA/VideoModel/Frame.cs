using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.VideoModel
{
	public class Frame
	{
		public int Height
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public int Width
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

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
			get { return Data[y * Width + x]; }
			set { Data[y * Width + x] = value; }
		}
	}
}
