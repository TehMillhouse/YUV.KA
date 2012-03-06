using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.VideoModel;

namespace Circuitry
{
	[DataContract]
	public abstract class DigitalNode : Node
	{
		static readonly Frame Zero = null;
		static readonly Frame One = new Frame(size: new Size(-42, -43));

		public static bool FrameToBool(Frame f) { return f == One; }
		public static Frame BoolToFrame(bool b) { return b ? One : Zero; }

		public DigitalNode(int? inputCount, int? outputCount) : base(inputCount, outputCount) { }

		public sealed override Frame[] Process(Frame[] inputs, int tick)
		{
			return ProcessDigital(inputs.Select(FrameToBool).ToArray()).Select(BoolToFrame).ToArray();
		}

		public abstract bool[] ProcessDigital(bool[] inputs);
	}

	[DataContract]
	public abstract class DigitalInputNode : InputNode
	{
		public DigitalInputNode(int? outputCount) : base(outputCount) { }

		[Browsable(false)]
		public Size Size { get; private set; }

		public sealed override Frame OutputFrame(int tick)
		{
			return DigitalNode.BoolToFrame(OutputDigital(tick));
		}

		public abstract bool OutputDigital(int tick);
	}

	[DataContract]
	public abstract class DigitalOutputNode : OutputNode
	{
		public DigitalOutputNode(int? inputCount) : base(inputCount) { }

		public sealed override void ProcessCore(Frame[] inputs, int tick)
		{
			ProcessDigital(inputs.Select(DigitalNode.FrameToBool).ToArray(), tick);
		}

		public abstract void ProcessDigital(bool[] inputs, int tick);
	}

	[DataContract]
	public class BitstreamInputNode : DigitalInputNode
	{
		public BitstreamInputNode()
			: base(outputCount: 1)
		{
			Name = "Bitstream Input";
			Input = new bool[0];
		}

		[DataMember]
		[Browsable(true)]
		public bool[] Input { get; set; }

		public override int? TickCount { get { return Input.Length; } }

		public override bool OutputDigital(int tick)
		{
			return tick < Input.Length ? Input[tick] : false;
		}
	}

	[DataContract]
	public class BitstreamOutputNode : DigitalOutputNode
	{
		ObservableCollection<bool?> output;

		public BitstreamOutputNode()
			: base(inputCount: 1)
		{
			Name = "Bitstream Output";
			output = new ObservableCollection<bool?>();
			Output = new ReadOnlyObservableCollection<bool?>(output);
		}

		[Browsable(true)]
		public ReadOnlyObservableCollection<bool?> Output { get; private set; }

		public override bool ProcessNodeInBackground { get { return true; } }

		public override void ProcessDigital(bool[] inputs, int tick)
		{
			if (tick != output.Count) {
				output.Clear();
				Enumerable.Range(0, tick).Select(_ => (bool?)null).Apply(output.Add);
			}
			output.Add(inputs[0]);
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext ctx)
		{
			output = new ObservableCollection<bool?>();
			Output = new ReadOnlyObservableCollection<bool?>(output);
		}
	}

	[DataContract]
	public class NopeNode : DigitalNode
	{
		public NopeNode()
			: base(1, 1)
		{
			Name = "Nope";
		}

		public override bool[] ProcessDigital(bool[] inputs)
		{
			return new[] { !inputs[0] };
		}
	}

	public enum BinaryOp { And, Or, Nor, Nand, Xor, Eq }

	[DataContract]
	public class BinaryNode : DigitalNode
	{
		public BinaryNode()
			: base(2, 1)
		{
			Name = "Binary";
		}

		[DataMember]
		[Browsable(true)]
		public BinaryOp Operation { get; set; }

		public override bool[] ProcessDigital(bool[] inputs)
		{
			bool a = inputs[0], b = inputs[1];
			return new[] { 
				Operation == BinaryOp.And ? a & b :
				Operation == BinaryOp.Or ? a | b :
				Operation == BinaryOp.Nor ? !(a | b) :
				Operation == BinaryOp.Nand ? !(a & b) :
				Operation == BinaryOp.Xor ? a ^ b :
				Operation == BinaryOp.Eq ? a == b :
				false
			};
		}
	}
}
