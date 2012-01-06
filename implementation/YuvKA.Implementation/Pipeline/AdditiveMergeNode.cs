using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class AdditiveMergeNode : Node
	{
		public AdditiveMergeNode()
			: base(inputCount: null, outputCount: 1)
		{
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			int maxX = 0;
			int maxY = 0;
			for (int i = 0; i < inputs.Length; i++) {
				maxX = Math.Max(maxX, inputs[i].Size.Width);
				maxY = Math.Max(maxY, inputs[i].Size.Height);
			}
			Frame[] output = { new Frame(new Size(maxX, maxY)) };
			for (int i = 0; i < inputs.Length; i++) {
				for (int x = 0; x < maxX; x++) {
					for (int y = 0; y < maxY; y++) {
						byte newR = (byte)Math.Min(255, output[0][x, y].R + GetPixel(x, y, inputs[i]).R);
						byte newG = (byte)Math.Min(255, output[0][x, y].G + GetPixel(x, y, inputs[i]).G);
						byte newB = (byte)Math.Min(255, output[0][x, y].B + GetPixel(x, y, inputs[i]).B);
						output[0][x, y] = new Rgb(newR, newG, newB);
					}
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
