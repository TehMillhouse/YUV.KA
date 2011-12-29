using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class NoiseInputNode : InputNode
	{
		public NoiseInputNode() : base(outputCount: 1)
		{
		}

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
