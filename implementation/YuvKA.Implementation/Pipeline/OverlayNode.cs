using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class OverlayNode : OutputNode
	{
		public OverlayNode() : base(inputCount: 2)
		{
		}

		[DataMember]
		public IOverlayType Type { get; set; }

		[DataMember]
		[Browsable(false)]
		public Frame Data { get; private set; }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			Data = Type.Process(inputs[0], (Type.DependsOnReference) ? inputs[1] : null);
		}
	}
}
