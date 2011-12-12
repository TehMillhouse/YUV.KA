using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class VideoInputNode : InputNode
	{
		[Browsable(false)]
		YuvEncoder.Video input;

		[DisplayName("Video File")]
		[DataMember]
		public FilePath FileName { get; set; }

		[DisplayName("Optional Log File")]
		[DataMember]
		public FilePath LogFileName { get; set; }

		[Browsable(false)]
		public override int TickCount { get { return input.FrameCount; } }

		public override Frame[] ProcessFrame(Frame[] inputs, int frameIndex)
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
