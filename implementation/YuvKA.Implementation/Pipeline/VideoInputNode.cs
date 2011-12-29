using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class VideoInputNode : InputNode
	{
		YuvEncoder.Video input = new YuvEncoder.Video(); // place holder

		public VideoInputNode() : base(1)
		{
		}

		[DisplayName("Video File")]
		[DataMember]
		public FilePath FileName { get; set; }

		[DisplayName("Optional Log File")]
		[DataMember]
		public FilePath LogFileName { get; set; }

		[Browsable(false)]
		public override int TickCount { get { return input.FrameCount; } }

		public override Frame OutputFrame(int tick)
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			base.Dispose();
			if (input != null)
				input.Dispose();
		}
	}
}
