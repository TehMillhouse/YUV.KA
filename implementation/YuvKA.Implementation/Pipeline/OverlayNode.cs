using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
    [DataContract]
	public class OverlayNode : OutputNode
	{
		public OverlayNode() : base(1)
		{
		}

        [DataMember]
		public IOverlayType Type { get; set; }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
