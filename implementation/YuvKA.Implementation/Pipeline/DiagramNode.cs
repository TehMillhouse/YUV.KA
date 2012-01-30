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
			Name = "Diagram";
			IsEnabled = true;
			Graphs = new List<DiagramGraph>();
		}

		[DataMember]
		[DisplayName("Enabled")]
		[Browsable(true)]
		public bool IsEnabled { get; set; }

		[DataMember]
		public Input ReferenceVideo { get; set; }

		[DataMember]
		public List<DiagramGraph> Graphs { get; set; }

		[Browsable(true)]
		public DiagramViewModel Window { get { return new DiagramViewModel(this); } }

		private int RefIndex
		{
			get { return Inputs.IndexOf(ReferenceVideo); }
		}

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			foreach (DiagramGraph g in Graphs) {
				if (!(g.Data.Count == 0) && tick < g.Data[g.Data.Count - 1].Key)
					g.Data = new List<KeyValuePair<int, double>>();
				g.Data.Add(new KeyValuePair<int, double>(tick, g.Type.Process(inputs[Inputs.IndexOf(g.Video)], inputs[RefIndex])));
			}
		}
	}
}
