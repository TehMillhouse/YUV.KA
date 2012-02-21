using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class implements the possibility to split a Frame into its RGB components.
	/// </summary>
	[DataContract]
	public class RgbSplitNode : Node
	{
		/// <summary>
		/// Construct a RgbSplitNode.
		/// It has one Input and three Outputs.
		/// </summary>
		public RgbSplitNode()
			: base(inputCount: 1, outputCount: 3)
		{
			Name = "RGB-Split";
		}

		/// <summary>
		/// Splits the first entry of the input into its RGB components
		/// </summary>
		/// <param name="inputs">An array of Frames, with only the first entry regarded.</param>
		/// <param name="tick">The index of the Frame which is processes now.</param>
		/// <returns>An array of Frames with the red components of the input in the first,
		/// the green component in the second and the blue component in the third entry. </returns>
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
