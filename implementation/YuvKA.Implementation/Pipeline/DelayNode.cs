using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DelayNode : Node
	{
		Queue<Frame> queue = new Queue<Frame>();

		public DelayNode() : base(inputCount: 1, outputCount: 1)
		{
		}

		[DataMember]
		[Range(0, 10)]
		public int Delay { get; set; }

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
