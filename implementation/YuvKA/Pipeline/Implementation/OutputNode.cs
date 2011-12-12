using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public abstract class OutputNode : Node
	{
		public sealed override Frame[] ProcessFrame(Frame[] inputs, int frameIndex)
		{
			ProcessFrameCore(inputs, frameIndex);
			return new Frame[] { };
		}

		public abstract void ProcessFrameCore(Frame[] inputs, int frameIndex);
	}
}
