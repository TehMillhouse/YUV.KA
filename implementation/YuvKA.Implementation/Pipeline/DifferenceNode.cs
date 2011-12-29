using System;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class DifferenceNode : Node
	{
		public DifferenceNode() : base(inputCount: 2, outputCount: 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
