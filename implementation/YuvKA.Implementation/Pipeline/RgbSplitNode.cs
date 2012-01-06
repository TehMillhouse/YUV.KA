using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class RgbSplitNode : Node
	{
		public RgbSplitNode()
			: base(inputCount: 1, outputCount: 3)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
