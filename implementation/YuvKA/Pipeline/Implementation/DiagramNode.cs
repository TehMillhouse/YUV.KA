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
	public class DiagramNode : OutputNode
	{
		[DataMember]
		[DisplayName("Enabled")]
		public bool IsEnabled { get; set; }

		[DataMember]
		[Browsable(false)]
		public int? ReferenceVideo
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

		public override void ProcessFrameCore(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
