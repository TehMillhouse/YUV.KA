using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DiagramNode : OutputNode
	{
		public DiagramNode(Input reference)
			: base(inputCount: null)
		{
		    Inputs.Add(reference);
			IsEnabled = true;
			ReferenceVideo = reference;
			Graphs = new List<DiagramGraph>();
		}

		[DataMember]
		[DisplayName("Enabled")]
		public bool IsEnabled { get; set; }


		[DataMember]
		[Browsable(false)]
		public Input ReferenceVideo { get; set; }

		private int RefIndex
		{
			get { return Inputs.IndexOf(ReferenceVideo); }
		}

		[DataMember]
		[Browsable(false)]
		public List<DiagramGraph> Graphs { get; set; }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			foreach (DiagramGraph g in Graphs) {
				g.Data.Add(g.Type.Process(inputs[Inputs.IndexOf(g.Video)], inputs[RefIndex]));
			}
		}
	}
}
