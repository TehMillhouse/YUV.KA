using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuvKA.VideoModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
namespace YuvKA.Pipeline
{
	[InheritedExport]
	[DataContract]
	public abstract class Node : IDisposable
	{
		public class Input
		{
			public Output Source { get; set; }
			public int Index { get; private set; }
		}

		public class Output
		{
			public Node Node { get; private set; }
			public int Index { get; private set; }
			public bool OutputsLog { get; set; }

		}

		[Browsable(false)]
		[DataMember]
		public double X { get; set; }
		[Browsable(false)]
		[DataMember]
		public double Y { get; set; }

		[Browsable(false)]
		public ICollection<Input> Inputs { get; private set; }

		[Browsable(false)]
		public ICollection<Output> Outputs { get; private set; }

		public abstract Frame[] ProcessFrame(Frame[] inputs, int tick);

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion
	}
}
