using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class InverterNode : Node
	{
		public InverterNode()
			: base(inputCount: 1, outputCount: 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Frame[] result = { new Frame(inputs[0].Size) };
			for (int x = 0; x < inputs[0].Size.Width; x++) {
				for (int y = 0; y < inputs[0].Size.Height; y++) {
					byte newR = (byte)(255 - inputs[0][x, y].R);
					byte newG = (byte)(255 - inputs[0][x, y].G);
					byte newB = (byte)(255 - inputs[0][x, y].B);
					result[0][x, y] = new Rgb(newR, newG, newB);
				}
			}
			return result;
		}
	}
}
