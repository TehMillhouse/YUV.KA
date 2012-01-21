using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class DifferenceNode : Node
	{
		public DifferenceNode()
			: base(inputCount: 2, outputCount: 1)
		{
			Name = "Difference";
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Size maxSize = Frame.MaxBoundaries(inputs);
			Frame[] output = { new Frame(new Size(maxSize.Width, maxSize.Height)) };
			for (int x = 0; x < maxSize.Width; x++) {
				for (int y = 0; y < maxSize.Height; y++) {
					byte newR = (byte)(127 + (((int)inputs[0].GetPixelOrBlack(x, y).R - inputs[1].GetPixelOrBlack(x, y).R) / 2));
					byte newG = (byte)(127 + (((int)inputs[0].GetPixelOrBlack(x, y).G - inputs[1].GetPixelOrBlack(x, y).G) / 2));
					byte newB = (byte)(127 + (((int)inputs[0].GetPixelOrBlack(x, y).B - inputs[1].GetPixelOrBlack(x, y).B) / 2));
					output[0][x, y] = new Rgb(newR, newG, newB);
				}
			}
			return output;
		}
	}
}
