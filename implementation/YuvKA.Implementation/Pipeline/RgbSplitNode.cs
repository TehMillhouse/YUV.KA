using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class RgbSplitNode : Node
	{
		public RgbSplitNode()
			: base(inputCount: 1, outputCount: 3)
		{
			Name = "RGB-Split";
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Size size = inputs[0].Size;
			Frame[] outputs = { new Frame(size), new Frame(size), new Frame(size) };
			for (int x = 0; x < size.Width; x++) {
				for (int y = 0; y < size.Height; y++) {
					outputs[0][x, y] = new Rgb(inputs[0][x, y].R, 0, 0);
					outputs[1][x, y] = new Rgb(0, inputs[0][x, y].G, 0);
					outputs[2][x, y] = new Rgb(0, 0, inputs[0][x, y].B);
				}
			}
			return outputs;
		}
	}
}
