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
	public abstract class Node
	{
		/// <summary>
		/// Initializes the Node object. Parameters hold the number of inputs or outputs to add, respectively,
		/// or null if the count should be variable. In the latter case a resizable collection of initial size
		/// 0 will be created.
		/// </summary>
		public Node(int? inputCount, int? outputCount)
		{
			if (inputCount.HasValue)
				Inputs = Enumerable.Range(0, inputCount.Value).Select(_ => new Input()).ToArray();
			else {
				Inputs = new ObservableCollection<Input>();
				UserCanAddInputs = true;
			}

			if (outputCount.HasValue)
				Outputs = Enumerable.Range(0, outputCount.Value).Select(_ => new Output(this)).ToArray();
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
		public virtual bool InputIsValid
		{
			get
			{
				return Inputs.All(input => input.Source != null && !input.Source.Node.InputIsValid);
			}
		}

		/// <summary>
		/// Represents the number of frames the node needs to precompute before he can process the frame of the current tick.
		/// </summary>
		[Browsable(false)]
		public virtual int NumberOfFramesToPrecompute
		{
			get { return 0; }
		}

		[DataMember]
		[Browsable(false)]
		public IList<Input> Inputs { get; private set; }

		[DataMember]
		[Browsable(false)]
		public bool UserCanAddInputs { get; private set; }

		[DataMember]
		[Browsable(false)]
		public IList<Output> Outputs { get; private set; }

		public abstract Frame[] Process(Frame[] inputs, int tick);

		[DataContract]
		public class Input
		{
			[DataMember]
			public Output Source { get; set; }
		}

		[DataContract]
		public class Output
		{
			public Output(Node node)
			{
				Node = node;
			}

			[DataMember]
			public Node Node { get; private set; }
		}
	}
}
