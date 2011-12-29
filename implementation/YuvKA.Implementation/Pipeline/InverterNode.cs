using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class InverterNode : Node
	{
		public InverterNode() : base(inputCount: 1, outputCount: 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
