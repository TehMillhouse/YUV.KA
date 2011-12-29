using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class AdditiveMergeNode : Node
	{
		public AdditiveMergeNode() : base(inputCount: null, outputCount: 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
