namespace YuvKA.Implementation
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;
	using YuvKA;
	using YuvKA.VideoModel;
	using System.Runtime.Serialization;

	public class SingleFrameNode : YuvKA.Pipeline.Node
	{
		public SingleFrameNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Name = "Single Frame";
		}

		[Browsable(true)]
		[DisplayName("Frame Number")]
		[Range(0.0, 100.0)]
		[DataMember]
		public int frameNum { get; set; }
		private Frame blackFrame = new Frame(new Size(100, 100));

		public override YuvKA.VideoModel.Frame[] Process(Frame[] inputs, int tick)
		{
			if (tick == frameNum)
				return inputs;
			else
				return new Frame[] { blackFrame };

		}
	}
}
