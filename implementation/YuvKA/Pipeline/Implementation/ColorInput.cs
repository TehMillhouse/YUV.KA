using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class ColorInputNode : InputNode
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

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
