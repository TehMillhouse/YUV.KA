using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class ColorInputNode : INode
	{
		public RGB Color
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
		#region INode Members

		public Frame[] Process(Frame[] input)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
