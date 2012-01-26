using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DiagramGraph
	{
		public DiagramGraph()
		{
			Data = new List<KeyValuePair<int, double>>();
		}


		[DataMember]
		public YuvKA.Pipeline.Node.Input Video { get; set; }

		[DataMember]
		public IGraphType Type { get; set; }

		[DataMember]
		public IList<KeyValuePair<int, double>> Data { get; set; }
	}
}
