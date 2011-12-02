using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.Pipeline.Implementation
{
	public abstract class InputNode : Node
	{
		public int Height { get; set; }
		public int Width { get; set; }
		public virtual int FrameCount { get { return 1; } }
	}
}
