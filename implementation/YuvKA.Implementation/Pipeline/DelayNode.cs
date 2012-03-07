using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Implements the possibility to delay a Frame.
	/// </summary>
	[DataContract]
	[Description("This Node delays the input for a variable amount of frames")]
	public class DelayNode : Node
	{
		Queue<Frame> queue;
		int lastTick;

		/// <summary>
		/// Constructs a delaynode with a delay of 0.
		/// It has one In- and one Output.
		/// </summary>
		public DelayNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Name = "Delay";
		}

		/// <summary>
		/// The number of indices the Frames shall be delayed.
		/// </summary>
		[DataMember]
		[Range(0.0, 30.0)]
		[Browsable(true)]
		public int Delay { get; set; }

		/// <summary>
		/// The value if the output Frames have vectordata attached.
		/// </summary>
		public override bool OutputHasMotionVectors
		{
			get { return Inputs.All(input => input.Source != null && input.Source.Node.OutputHasMotionVectors); }
		}

		/// <summary>
		/// The value if the output Frames have logdata attached.
		/// </summary>
		public override bool OutputHasLogfile
		{
			get
			{
				return Inputs.All(input => input.Source != null && input.Source.Node.OutputHasLogfile);
			}
		}

		/// <summary>
		/// Delays the input.
		/// </summary>
		/// <param name="inputs">An array of Frames, with only the first entry regarded.</param>
		/// <param name="tick">The index of the Frame which is processes now.</param>
		/// <returns>An array of Frames, whose only entry is the input that was processed Delay functions calls ago.
		/// If no such input is available that only entry is a black Frame.</returns>
		public override Frame[] Process(Frame[] inputs, int tick)
		{
			if (queue == null)
				queue = new Queue<Frame>();
			if (tick != lastTick + 1)
				queue.Clear();

			queue.Enqueue(inputs[0]);
			while (queue.Count > Delay + 1)
				queue.Dequeue();

			lastTick = tick;
			return new[] { queue.Count == Delay + 1 ? queue.Dequeue() : queue.First() };
		}
	}
}
