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
        [DataMember]
		public IOverlayType Type { get; set; }

		public override void ProcessFrameCore(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
