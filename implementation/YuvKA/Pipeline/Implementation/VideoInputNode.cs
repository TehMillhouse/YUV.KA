using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class VideoInputNode : InputNode
	{
		YuvEncoder.Video input;

		public string FileName { get; set; }
		public string LogFileName { get; set; }

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}

		public override int FrameCount { get { return input.FrameCount; } }

		public override void Dispose()
		{
			base.Dispose();
			if (input != null) input.Dispose();
		}
	}
}
