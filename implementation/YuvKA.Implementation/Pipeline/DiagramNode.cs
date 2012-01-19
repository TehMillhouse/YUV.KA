using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DiagramNode : OutputNode
	{
		public DiagramNode()
			: base(inputCount: null)
		{
			IsEnabled = true;
			Graphs = new List<DiagramGraph>();
		}

		[DataMember]
		[DisplayName("Enabled")]
		public bool IsEnabled { get; set; }


		[DataMember]
		[Browsable(false)]
		public Input ReferenceVideo { get; set; }

		[DataMember]
		[Browsable(false)]
		public List<DiagramGraph> Graphs { get; set; }

		public DiagramViewModel Window { get { return new DiagramViewModel(this); } }

		private int RefIndex
		{
			get { return Inputs.IndexOf(ReferenceVideo); }
		}

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			foreach (DiagramGraph g in Graphs) {
				g.Data.Add(g.Type.Process(inputs[Inputs.IndexOf(g.Video)], inputs[RefIndex]));
			}
		}
	}
}
