using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DelayNode : Node
	{
		Queue<Frame> queue;

		public DelayNode()
			: base(inputCount: 1, outputCount: 1)
		{
		}

		[DataMember]
		[Range(0.0, 10.0)]
		public int Delay { get; set; }

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			if (queue == null) {
				Frame blackFrame = new Frame(inputs[0].Size);
				for (int x = 0; x < inputs[0].Size.Width; x++) {
					for (int y = 0; y < inputs[0].Size.Height; y++) {
						blackFrame[x, y] = new Rgb(0, 0, 0);
					}
				}
				queue = new Queue<Frame>();
				for (int i = 0; i < Delay; i++) {
					queue.Enqueue(blackFrame);
				}
			}
			queue.Enqueue(inputs[0]);
			Frame[] returnFrame = { queue.Dequeue() };
			return returnFrame;
		}
	}
}
