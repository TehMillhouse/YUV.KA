using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class implements the possibility to subtract one Frame from another.
	/// </summary>
	[DataContract]
	public class DifferenceNode : Node
	{
		/// <summary>
		/// Construct a differencenode.
		/// It has two Inputs and one Output.
		/// </summary>
		public DifferenceNode()
			: base(inputCount: 2, outputCount: 1)
		{
			Name = "Difference";
		}

		/// <summary>
		/// Subtracts the second entry of input from the first entry.
		/// If the first Frame has a greater size than the second one, the missing data will be simulated with black.
		/// </summary>
		/// <param name="inputs">An array of Frames, with only the first two entries regarded.</param>
		/// <param name="tick">The index of the Frame which is processes now.</param>
		/// <returns>An array of Frames whose only entry is the subtraction of the second entry of input from the first entry.</returns>
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
