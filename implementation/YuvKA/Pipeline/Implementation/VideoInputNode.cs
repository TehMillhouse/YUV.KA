using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;
using System.ComponentModel;

namespace YuvKA.Pipeline.Implementation
{
	public class VideoInputNode : InputNode
	{
		YuvEncoder.Video input;

		[DisplayName("Video File")]
		public FilePath FileName { get; set; }

		[DisplayName("Optional Log File")]
		public FilePath LogFileName { get; set; }

		public override int FrameCount { get { return input.FrameCount; } }

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			base.Dispose();
			if (input != null) input.Dispose();
		}
	}
}
