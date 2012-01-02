using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class VideoInputNode : InputNode
	{
		FilePath fileName = new FilePath(null);
		FilePath logFileName = new FilePath(null);
		YuvEncoder.Video input;

		public VideoInputNode()
			: base(outputCount: 1)
		{
		}

		[DisplayName("Video File")]
		[DataMember]
		public FilePath FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				input = null;
			}
		}

		[DisplayName("Optional Log File")]
		[DataMember]
		public FilePath LogFileName
		{
			get { return logFileName; }
			set
			{
				logFileName = value;
				input = null;
			}
		}

		[Browsable(false)]
		public override int TickCount { get { return input.FrameCount; } }

		public override Frame OutputFrame(int tick)
		{
			if (input == null)
				input = YuvEncoder.Decode(FileName.Path, LogFileName.Path, Size.Width, Size.Height);

			if (tick < 0 || tick >= input.FrameCount)
				return new Frame(Size);

			return input[tick];
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			input = null;
		}
	}
}
