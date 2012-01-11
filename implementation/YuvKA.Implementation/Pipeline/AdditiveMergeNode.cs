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
			Size maxSize = Frame.MaxBoundaries(inputs);
			Frame[] output = { new Frame(new Size(maxSize.Height, maxSize.Width)) };
			for (int i = 0; i < inputs.Length; i++) {
				for (int x = 0; x < maxSize.Width; x++) {
					for (int y = 0; y < maxSize.Height; y++) {
						byte newR = (byte)Math.Min(255, output[0][x, y].R + inputs[i].GetPixelOrBlack(x, y).R);
						byte newG = (byte)Math.Min(255, output[0][x, y].G + inputs[i].GetPixelOrBlack(x, y).G);
						byte newB = (byte)Math.Min(255, output[0][x, y].B + inputs[i].GetPixelOrBlack(x, y).B);
						output[0][x, y] = new Rgb(newR, newG, newB);
					}
				}
			}
			return output;
		}
	}
}
