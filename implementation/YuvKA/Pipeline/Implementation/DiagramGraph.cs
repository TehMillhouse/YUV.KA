using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DiagramGraph
	{
		[DataMember]
		public YuvKA.Pipeline.Node.Input Video
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
		public IGraphType Type
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
		public IList<double> Data
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
	}
}
