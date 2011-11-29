using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class ColorInputNode : Node
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

		public override void ProcessFrame()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
