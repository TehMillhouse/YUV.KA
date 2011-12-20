using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
namespace YuvKA.Pipeline
{
	[InheritedExport]
	[DataContract]
	public abstract class Node : IDisposable
	{
		[Browsable(false)]
		[DataMember]
		public double X { get; set; }

		[Browsable(false)]
		[DataMember]
		public double Y { get; set; }

		[Browsable(false)]
		public IList<Input> Inputs { get; private set; }

		[Browsable(false)]
		public IList<Output> Outputs { get; private set; }

		public abstract Frame[] Process(Frame[] inputs, int tick);

		public virtual void Dispose()
		{
		}

		public class Input
		{
			public Output Source { get; set; }
			public int Index { get; private set; }
		}

		public class Output
		{
			public Node Node { get; private set; }
			public int Index { get; private set; }
		}
	}
}
