using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class VideoInputNode : InputNode
	{
		static readonly Size Cif = new Size(352, 288);
		static readonly Size Sif = new Size(352, 240);

		FilePath fileName = new FilePath(null);
		FilePath logFileName = new FilePath(null);
		FilePath motionVectorFileName = new FilePath(null);
		YuvEncoder.Video input;

		public VideoInputNode()
			: base(outputCount: 1)
		{
		}

		[DisplayName("Video File")]
		[DataMember]
		[Browsable(true)]
		public FilePath FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				input = null;

				if (value.Path != null) {
					// Try and guess the video resolution
					Match m = Regex.Match(fileName.Path, @"(\d+)x(\d+)");
					if (m.Success)
						Size = new Size(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
					else if (Regex.IsMatch(fileName.Path, @"[_.]cif[_.]"))
						Size = Cif;
					else if (Regex.IsMatch(fileName.Path, @"[_.]sif[_.]"))
						Size = Sif;
					NotifyOfPropertyChange(() => Size);
				}
			}
		}

		[DisplayName("(Log File)")]
		[DataMember]
		[Browsable(true)]
		public FilePath LogFileName
		{
			get { return logFileName; }
			set
			{
				logFileName = value;
				input = null;
			}
		}

		[DisplayName("(Motion Vector File)")]
		[DataMember]
		[Browsable(true)]
		public FilePath MotionVectorFileName
		{
			get { return motionVectorFileName; }
			set
			{
				motionVectorFileName = value;
				input = null;
			}
		}

		public override int TickCount
		{
			get
			{
				if (InputIsValid) {
					EnsureInputLoaded();
					return input.FrameCount;
				}
				return 0;
			}
		}

		public override bool InputIsValid
		{
			get
			{
				return File.Exists(fileName.Path) && (logFileName.Path == null || File.Exists(logFileName.Path));
			}
		}

		public override Frame OutputFrame(int tick)
		{
			EnsureInputLoaded();

			if (tick < 0 || tick >= input.FrameCount)
				return new Frame(Size);

			return input[tick];
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			input = null;
		}

		private void EnsureInputLoaded()
		{
			if (input == null)
				input = YuvEncoder.Decode(Size.Width, Size.Height, FileName.Path, LogFileName.Path, MotionVectorFileName.Path);
		}
	}
}
