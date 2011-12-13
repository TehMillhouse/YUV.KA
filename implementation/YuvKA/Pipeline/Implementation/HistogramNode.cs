using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{	
	[DataContract]
	public class HistogramNode : OutputNode
	{
		[DataMember]
		[Browsable(false)]
		public HistogramType Type
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		[DataMember]
		[Browsable(false)]
		public double[] Data { get; private set; }

		public override void ProcessFrameCore(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
