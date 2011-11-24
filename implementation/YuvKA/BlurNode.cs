using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class BlurNode : INode
	{
		public BlurType Type
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public int Radius
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
