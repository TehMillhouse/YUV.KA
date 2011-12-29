using System;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class DifferenceNode : Node
	{
		public DifferenceNode() : base(2, 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
