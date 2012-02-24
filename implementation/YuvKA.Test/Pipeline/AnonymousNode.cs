using System;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.Pipeline;
using YuvKA.VideoModel;

namespace YuvKA.Test.Pipeline
{
	/// <summary>
	/// A simple Node subclass to quickly define Node stubs using anonymous methods.
	/// </summary>
	[DataContract]
	class AnonymousNode : Node
	{
		Func<Frame[], int, Frame[]> process;

		public AnonymousNode(Func<Frame[], int, Frame[]> process, int outputCount, params Output[] inputs)
			: base(inputs.Length, outputCount)
		{
			this.process = process;
			for (int i = 0; i < inputs.Length; i++)
				Inputs[i].Source = inputs[i];
		}

		public AnonymousNode(Func<Frame[], int, Frame> process, params Output[] inputs)
			: this((frames, tick) => new[] { process(frames, tick) }, 1, inputs) { }

		public AnonymousNode(Action process, params AnonymousNode[] inputs)
			: this((frames, tick) => { process(); return new[] { new Frame(new Size(0, 0)) }; }, 1, inputs.Select(n => n.Outputs[0]).ToArray()) { }

		public AnonymousNode(params AnonymousNode[] inputs)
			: this(() => { throw new NotImplementedException(); }, inputs) { }

		public override int NumberOfTicksToPrecompute
		{
			get
			{
				return SettableNumberOfFramesToPrecompute;
			}
		}

		public int SettableNumberOfFramesToPrecompute { get; set; }

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			return process(inputs, tick);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// An AnonymousNode subclass that takes in multiple ints and returns a single one.
	/// </summary>
	class AnonIntNode : AnonymousNode
	{
		public AnonIntNode(Func<int[], int, int> process, params AnonIntNode[] inputs)
			: base((frames, tick) => new[] { new Frame(new Size(process(frames.Select(f => f.Size.Width).ToArray(), tick), 42)) }, 1, inputs.Select(n => n.Outputs[0]).ToArray()) { }

		public AnonIntNode(Func<int[], int> process, params AnonIntNode[] inputs)
			: base((frames, tick) => new[] { new Frame(new Size(process(frames.Select(f => f.Size.Width).ToArray()), 42)) }, 1, inputs.Select(n => n.Outputs[0]).ToArray()) { }
	}
}
