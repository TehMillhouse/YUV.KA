using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class InverterNode : Node
	{
		public InverterNode()
			: base(inputCount: 1, outputCount: 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
