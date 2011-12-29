using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class BlurNode : Node
	{
		public BlurNode() : base(1, 1)
		{
		}

		[DataMember]
		public BlurType Type { get; set; }

		[DataMember]
		[Range(0.0, double.PositiveInfinity)]
		public int Radius { get; set; }

		#region INode Members

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
