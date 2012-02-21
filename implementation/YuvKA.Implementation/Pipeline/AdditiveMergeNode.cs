using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class implements the possibility to overlay Frames additive.
	/// </summary>
	[DataContract]
	public class AdditiveMergeNode : Node
	{
		/// <summary>
		/// Constructs an AdditiveMergeNode.
		/// It has a variable number of Inputs and one Output.
		/// </summary>
		public AdditiveMergeNode()
			: base(inputCount: null, outputCount: 1)
		{
			Name = "Additive Merge";
		}

		/// <summary>
		/// Creates an additive overlay of all inputframes.
		/// If the frames have different sizes, the maximum width and height are taken and missing data is simulated with black.
		/// </summary>
		/// <param name="inputs">The array of Frames which will be added together.</param>
		/// <param name="tick">The index of the Frame which is processes now.</param>
		/// <returns>A Framearray whose only Entry has the addition of all RGB values of all inputframes.</returns>
		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Size maxSize = Frame.MaxBoundaries(inputs);
			Frame[] output = { new Frame(new Size(maxSize.Width, maxSize.Height)) };
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
