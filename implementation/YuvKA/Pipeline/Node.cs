using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	[InheritedExport]
	[DataContract]
	public abstract class Node : PropertyChangedBase
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
			OutputHasLogfile = false;
			OutputHasMotionVectors = false;
			if (outputCount.HasValue)
				Outputs = Enumerable.Range(0, outputCount.Value).Select(_ => new Output(this)).ToArray();
			else
				Outputs = new ObservableCollection<Output>();
		}

		/// <summary>
		/// The name of the node. In general a one-word description of what the node does.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		public bool OutputHasLogfile { get; set; }
		public bool OutputHasMotionVectors { get; set; }

		/// <summary>
		/// The X coordinate of the position of this node.
		/// </summary>
		[DataMember]
		public double X { get; set; }

		/// <summary>
		/// The Y coordinate of the position of this node.
		/// </summary>
		[DataMember]
		public double Y { get; set; }

		/// <summary>
		/// Returns true if all of this node's inputs are directly or indirectly connected to input nodes.
		/// </summary>
		public virtual bool InputIsValid
		{
			get
			{
				return Inputs.All(input => input.Source != null && input.Source.Node.InputIsValid);
			}
		}

		/// <summary>
		/// Represents the number of frames the node needs to precompute before he can process the frame of the current tick.
		/// </summary>
		public virtual int NumberOfTicksToPrecompute
		{
			get { return 0; }
		}

		/// <summary>
		/// This node's collection of inputs.
		/// </summary>
		[DataMember]
		public IList<Input> Inputs { get; private set; }

		[DataMember]
		public bool UserCanAddInputs { get; private set; }

		/// <summary>
		/// This node's collection of outputs.
		/// </summary>
		[DataMember]
		public IList<Output> Outputs { get; private set; }

		public abstract Frame[] Process(Frame[] inputs, int tick);

		/// <summary>
		/// Represents an input of a node. An input is a connection to a predecessing node whose output is requested.
		/// </summary>
		[DataContract]
		public class Input
		{
			[DataMember]
			public Output Source { get; set; }
		}

		/// <summary>
		/// Represents an output of a node. An output is the connection point a node offers, so other nodes can be chained after this output's node.
		/// </summary>
		[DataContract]
		public class Output
		{
			/// <summary>
			/// Constructs an output that belongs to the specified node.
			/// </summary>
			public Output(Node node)
			{
				Node = node;
			}

			/// <summary>
			/// Represents the position of this output in this ouput's node's collection of outputs.
			/// </summary>
			public int Index
			{
				get
				{
					return this.Node.Outputs.IndexOf(this);
				}
			}

			[DataMember]
			public Node Node { get; private set; }
		}
	}
}
