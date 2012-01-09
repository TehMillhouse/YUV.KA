using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DiagramGraph
	{
		public DiagramGraph()
		{
			Data = new List<double>();
		}


		[DataMember]
		public YuvKA.Pipeline.Node.Input Video { get; set; }

		[DataMember]
		public IGraphType Type { get; set; }

		[DataMember]
		public IList<double> Data { get; set; }

	}
}
