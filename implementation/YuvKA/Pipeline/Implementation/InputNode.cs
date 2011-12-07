using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public abstract class InputNode : Node
	{
		public Size Size { get; set; }

		[Browsable(false)]
		public virtual int FrameCount { get { return 1; } }
	}
}
