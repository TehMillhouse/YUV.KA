using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class implements the possibility of inverting the colors of a Frame.
	/// </summary>
	[DataContract]
	public class InverterNode : Node
	{
		/// <summary>
		/// Creates an inverterNode.
		/// It has one In- and one Output.
		/// </summary>
		public InverterNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Name = "Inverter";
		}

		/// <summary>
		/// Inverts the Colors of the inputframes.
		/// </summary>
		/// <param name="inputs">An array of Frames, with only the first entry regarded.</param>
		/// <param name="tick">The index of the Frame which is processes now.</param>
		/// <returns>An array of Frames, whose only entry is the inverted version of the input.</returns>
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
