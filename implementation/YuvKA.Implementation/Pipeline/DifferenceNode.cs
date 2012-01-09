using System;
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
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			int maxX = (inputs[0].Size.Width > inputs[1].Size.Width) ? inputs[0].Size.Width : inputs[1].Size.Width;
			int maxY = (inputs[0].Size.Height > inputs[1].Size.Height) ? inputs[0].Size.Height : inputs[1].Size.Height;
			Frame[] output = { new Frame(new Size(maxX, maxY)) };
			for (int x = 0; x < maxX; x++) {
				for (int y = 0; y < maxY; y++) {
					byte newR = (byte)(127 + (((int)output[0][x, y].R - output[1][x, y].R) / 2));
					byte newG = (byte)(127 + (((int)output[0][x, y].G - output[1][x, y].G) / 2));
					byte newB = (byte)(127 + (((int)output[0][x, y].B - output[1][x, y].B) / 2));
					output[0][x, y] = new Rgb(newR, newG, newB);
				}
			}
			return output;
		}

		private Rgb GetPixel(int x, int y, Frame input)
		{
			return (x > input.Size.Width || y > input.Size.Height || x < 0 || y < 0) ? new Rgb(0, 0, 0) : input[x, y];
		}
	}
}
