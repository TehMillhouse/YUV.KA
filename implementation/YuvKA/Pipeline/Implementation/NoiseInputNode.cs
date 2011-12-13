using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class NoiseInputNode : InputNode
	{
		[DataMember]
		public NoiseType Type { get; set; }

		#region INode Members

		public override Frame OutputFrame(int tick)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
