using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	[InheritedExport]
	[DataContract]
	public abstract class Node : IDisposable
	{
		/// <summary>
		/// Initializes the Node object. Parameters hold the number of inputs or outputs to add, respectively,
		/// or null if the count should be variable. In the latter case a resizable collection of initial size
		/// 0 will be created.
		/// </summary>
		public Node(int? inputCount, int? outputCount)
		{
			if (inputCount.HasValue)
				Inputs = Enumerable.Range(0, inputCount.Value).Select(i => new Input(i)).ToArray();
			else
				Inputs = new ObservableCollection<Input>();

			if (outputCount.HasValue)
				Outputs = Enumerable.Range(0, outputCount.Value).Select(i => new Output(this, i)).ToArray();
			else
				Outputs = new ObservableCollection<Output>();
		}

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
			public Input(int index) { Index = index; }

			public Output Source { get; set; }
			public int Index { get; private set; }
		}

		public class Output
		{
			public Output(Node node, int index)
			{
				Node = node;
				Index = index;
			}

			public Node Node { get; private set; }
			public int Index { get; private set; }
		}
	}
}
