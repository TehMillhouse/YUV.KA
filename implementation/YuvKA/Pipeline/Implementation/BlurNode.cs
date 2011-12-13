using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel.DataAnnotations;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
    [DataContract]
	public class BlurNode : Node
	{
        [DataMember]
		public BlurType Type { get; set; }

        [DataMember]
		[Range(0.0, double.PositiveInfinity)]
		public int Radius  { get; set; }


		#region INode Members

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
