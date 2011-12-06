using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class HistogramNode : OutputNode
	{
		[Browsable(false)]
		public HistogramType Type
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[Browsable(false)]
		public int[] Data { get; private set; }

		public override Frame[] ProcessFrame(Frame[] inputs, int frameIndex)
		{
			throw new NotImplementedException();
		}
	}
}
