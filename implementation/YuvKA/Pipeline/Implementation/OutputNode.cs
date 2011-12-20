using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public abstract class OutputNode : Node
	{
		public sealed override Frame[] Process(Frame[] inputs, int tick)
		{
			ProcessCore(inputs, tick);
			return new Frame[] { };
		}

		public abstract void ProcessCore(Frame[] inputs, int tick);
	}
}
