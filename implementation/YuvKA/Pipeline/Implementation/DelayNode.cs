using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
    [DataContract]
	public class DelayNode : Node
	{
		Queue<Frame> queue = new Queue<Frame>();

        [DataMember]
		[Range(0, 10)]
		public int Delay { get; set; }

		public override Frame[] ProcessFrame(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
