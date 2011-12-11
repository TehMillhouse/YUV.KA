using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class BlurNode : Node
	{
		public BlurType Type { get; set; }

		[Range(0.0, double.PositiveInfinity)]
		public int Radius  { get; set; }


		#region INode Members

		public override Frame[] ProcessFrame(Frame[] inputs, int frameIndex)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
