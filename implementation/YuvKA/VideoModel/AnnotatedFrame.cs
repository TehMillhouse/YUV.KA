using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.VideoModel
{
	public class AnnotatedFrame : Frame
	{
		public AnnotatedFrame(Size size)
			: base(size)
		{
		}

		public MacroblockDecision[] Decisions
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
