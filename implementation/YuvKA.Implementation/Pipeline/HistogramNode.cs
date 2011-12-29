using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class HistogramNode : OutputNode
	{
		public HistogramNode() : base(inputCount: 1)
		{
		}

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

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
