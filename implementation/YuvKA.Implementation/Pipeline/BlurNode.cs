using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class implements the possibility to blur a given Frame.
	/// It has a Type and a Radius to define how the Frame shall be blurred.
	/// </summary>
	[DataContract]
	[Description("This Node can blur the input")]
	public class BlurNode : Node
	{
		// cache the weights, so they must be only calculated once per video(best case)
		Tuple<float[], int, BlurType> weights;

		/// <summary>
		/// Constructs a blurnode with initial values "Linear" and 1 for the Type and Radius.
		/// It has one In- and one Output.
		/// </summary>
		public BlurNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Name = "Blur";
			Type = BlurType.Linear;
			Radius = 1;
		}

		/// <summary>
		/// The type of blur which shall be applied.
		/// </summary>
		[DataMember]
		[Browsable(true)]
		public BlurType Type { get; set; }

		/// <summary>
		/// The radius of blur.
		/// </summary>
		[DataMember]
		[Range(0.0, 42.0)]
		[Browsable(true)]
		public int Radius { get; set; }

		/// <summary>
		/// Blurs the inputframes.
		/// </summary>
		/// <param name="inputs">An array of Frames, with only the first entry regarded.</param>
		/// <param name="tick">The index of the Frame which is processes now.</param>
		/// <returns>An array of Frames with one entry, which is the blurred version of the input.
		/// Returns the input, if the radius is 0.</returns>
		public override Frame[] Process(Frame[] inputs, int tick)
		{
			// no need to do anything with radius 0
			if (Radius == 0)
				return inputs;
			// if the cached weights arent valid recompute them
			if (weights == null || weights.Item2 != this.Radius || weights.Item3 != this.Type) {
				if (Type == BlurType.Linear) {
					CalculateLinearWeights(this.Radius);
				}
				else if (Type == BlurType.Gaussian) {
					CalculateGaussianWeights(this.Radius);
				}
			}
			Frame[] result = new Frame[1];
			if (Type == BlurType.Gaussian) {
				return new[] { GaussianBlur(inputs[0], tick) };
			}
			else if (Type == BlurType.Linear) {
				return new[] { LinearBlur(inputs[0], tick) };
			}
			// this should never happen
			return new Frame[] { null };
		}

		private Frame LinearBlur(Frame input, int tick)
		{
			// Since this Arrays are sort of ugly now: Arrayname[x-coord, y-coord, colorchannel]
			float[,,] horizontalBlur = new float[input.Size.Width, input.Size.Height, 3];
			float[,,] verticalBlur = new float[input.Size.Width, input.Size.Height, 3];
			// Cache Radius
			int cachedRadius = this.Radius;
			if (cachedRadius != this.weights.Item2)
				CalculateLinearWeights(this.Radius);
			// Blur horinzontal dimension
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					horizontalBlur[x, y, 0] = horizontalBlur[x, y, 1] = horizontalBlur[x, y, 2] = 0F;
					for (int z = x - cachedRadius; z <= x + cachedRadius; z++) {
						Rgb imagePixel = GetCappedPixels(z, y, input);
						horizontalBlur[x, y, 0] += weights.Item1[0] * imagePixel.R;
						horizontalBlur[x, y, 1] += weights.Item1[0] * imagePixel.G;
						horizontalBlur[x, y, 2] += weights.Item1[0] * imagePixel.B;
					}
				}
			}
			// Blur vertical dimension
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					verticalBlur[x, y, 0] = verticalBlur[x, y, 1] = verticalBlur[x, y, 2] = 0F;
					for (int z = y - cachedRadius; z <= y + cachedRadius; z++) {
						int cappedY = Math.Min(input.Size.Height - 1, Math.Max(0, z));
						verticalBlur[x, y, 0] += weights.Item1[0] * horizontalBlur[x, cappedY, 0];
						verticalBlur[x, y, 1] += weights.Item1[0] * horizontalBlur[x, cappedY, 1];
						verticalBlur[x, y, 2] += weights.Item1[0] * horizontalBlur[x, cappedY, 2];
					}
				}
			}
			// Convert floatarray to frame
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					result[x, y] = new Rgb((byte)verticalBlur[x, y, 0], (byte)verticalBlur[x, y, 1], (byte)verticalBlur[x, y, 2]);
				}
			}
			return result;
		}

		private Frame GaussianBlur(Frame input, int tick)
		{
			// Since this Arrays are sort of ugly now: Arrayname[x-coord, y-coord, colorchannel]
			float[,,] horizontalBlur = new float[input.Size.Width, input.Size.Height, 3];
			float[,,] verticalBlur = new float[input.Size.Width, input.Size.Height, 3];
			// Cache Radius
			int cachedRadius = this.Radius;
			if (cachedRadius != this.weights.Item2)
				CalculateGaussianWeights(cachedRadius);
			// Blur horinzontal dimension
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					horizontalBlur[x, y, 0] = horizontalBlur[x, y, 1] = horizontalBlur[x, y, 2] = 0F;
					for (int z = x - (3 * cachedRadius); z <= x + (3 * cachedRadius); z++) {
						Rgb imagePixel = GetCappedPixels(z, y, input);
						horizontalBlur[x, y, 0] += weights.Item1[Math.Abs(z - x)] * imagePixel.R;
						horizontalBlur[x, y, 1] += weights.Item1[Math.Abs(z - x)] * imagePixel.G;
						horizontalBlur[x, y, 2] += weights.Item1[Math.Abs(z - x)] * imagePixel.B;
					}
				}
			}
			// Blur vertical dimension
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					verticalBlur[x, y, 0] = verticalBlur[x, y, 1] = verticalBlur[x, y, 2] = 0F;
					for (int z = y - (3 * cachedRadius); z <= y + (3 * cachedRadius); z++) {
						int cappedY = Math.Min(input.Size.Height - 1, Math.Max(0, z));
						verticalBlur[x, y, 0] += weights.Item1[Math.Abs(z - y)] * horizontalBlur[x, cappedY, 0];
						verticalBlur[x, y, 1] += weights.Item1[Math.Abs(z - y)] * horizontalBlur[x, cappedY, 1];
						verticalBlur[x, y, 2] += weights.Item1[Math.Abs(z - y)] * horizontalBlur[x, cappedY, 2];
					}
				}
			}
			// Convert floatarray to frame
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					// Compensate the loss due to disregarding the pixels from 3 * Radius to infinity (for -x, -y, +x, +y)
					byte newR = (verticalBlur[x, y, 0] == 0) ? (byte)verticalBlur[x, y, 0] : (byte)(verticalBlur[x, y, 0] + 1);
					byte newG = (verticalBlur[x, y, 1] == 0) ? (byte)verticalBlur[x, y, 1] : (byte)(verticalBlur[x, y, 1] + 1);
					byte newB = (verticalBlur[x, y, 2] == 0) ? (byte)verticalBlur[x, y, 2] : (byte)(verticalBlur[x, y, 2] + 1);
					result[x, y] = new Rgb(newR, newG, newB);
				}
			}
			return result;
		}

		private float G(int x)
		{
			return (float)((1 / Math.Sqrt(2 * Math.PI * Radius * Radius)) * Math.Pow(Math.E, -1 * (((double)x * x) / (2 * Radius * Radius))));
		}

		private Rgb GetCappedPixels(int x, int y, Frame frame)
		{
			int cappedX = Math.Min(frame.Size.Width - 1, Math.Max(0, x));
			int cappedY = Math.Min(frame.Size.Height - 1, Math.Max(0, y));
			return frame[cappedX, cappedY];
		}

		private void CalculateGaussianWeights(int r)
		{
			float[] weight = new float[3 * r + 1];
			for (int i = 0; i <= r * 3; i++) {
				weight[i] = G(i);
			}
			weights = Tuple.Create(weight, r, this.Type);
		}

		private void CalculateLinearWeights(int r)
		{
			weights = Tuple.Create(new float[] { 1F / (2 * r + 1) }, r, this.Type);
		}
	}
}
