using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;
using System.Linq;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Provides a diagram representing information about the inputs graphically.
	/// </summary>
	[DataContract]
	public class DiagramNode : OutputNode
	{
		/// <summary>
		/// Creates a new diagram node. By default it is enabled and contains an empty
		/// list of graphs.
		/// </summary>
		public DiagramNode()
			: base(inputCount: null)
		{
			Name = "Diagram";
			IsEnabled = true;
			Graphs = new ObservableCollection<DiagramGraph>();
		}

		/// <summary>
		/// Specifies whether the DiagramNode is enabled.
		/// </summary>
		[DataMember]
		[DisplayName("Enabled")]
		[Browsable(true)]
		public bool IsEnabled { get; set; }

		public override bool ProcessNodeInBackground { get { return IsEnabled; } }

		/// <summary>
		/// Gets or sets the video to which all other videos are compared.
		/// </summary>
		[DataMember]
		public Input ReferenceVideo { get; set; }

		/// <summary>
		/// Gets or sets the list of graphs the DiagramNode is displaying.
		/// </summary>
		[DataMember]
		public ObservableCollection<DiagramGraph> Graphs { get; private set; }

		/// <summary>
		/// Returns a new DiagramViewModel corresponding to this DiagramNode.
		/// </summary>
		[Browsable(true)]
		public DiagramViewModel Window { get { return new DiagramViewModel(this); } }

		/// <summary>
		/// Returns the index of the ReferenceVideo in the list of Inputs of this DiagramNode.
		/// </summary>
		private int? RefIndex
		{
			get { return ReferenceVideo != null ? Inputs.IndexOf(ReferenceVideo) : (int?)null; }
		}

		/// <summary>
		/// Calculates the Data of each Graph for the current frame
		/// </summary>
		/// <param name="inputs">The frame to process.</param>
		/// <param name="tick">The index of the current frame. It is saved to the Data of the Graph.</param>
		public override void ProcessCore(Frame[] inputs, int tick)
		{
			if (!IsEnabled) {
				return;
			}

			foreach (DiagramGraph g in Graphs.ToArray()) {
				if (g.Data.Count != 0 && tick < g.Data[g.Data.Count - 1].Key)
					g.Data.Clear();
				g.Data.Add(new KeyValuePair<int, double>(tick,
					g.Type.Process(inputs[Inputs.IndexOf(g.Video)], ReferenceVideo != null ? inputs[(int)RefIndex] : null)));
			}
		}

		// Remove references to culled inputs
		public override void CullInputs()
		{
			base.CullInputs();

			if (!Inputs.Contains(ReferenceVideo))
				ReferenceVideo = null;

			foreach (DiagramGraph graph in Graphs.ToArray())
				if (!Inputs.Contains(graph.Video))
					Graphs.Remove(graph);
		}
	}
}