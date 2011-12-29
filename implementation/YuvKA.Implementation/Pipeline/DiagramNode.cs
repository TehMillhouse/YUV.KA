using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DiagramNode : OutputNode
	{
		public DiagramNode() : base(inputCount: null)
		{
		}

		[DataMember]
		[DisplayName("Enabled")]
		public bool IsEnabled { get; set; }

		[DataMember]
		[Browsable(false)]
		public Input ReferenceVideo
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[DataMember]
		[Browsable(false)]
		public List<DiagramGraph> Graphs
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
