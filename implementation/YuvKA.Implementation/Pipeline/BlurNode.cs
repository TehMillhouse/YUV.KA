using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class BlurNode : Node
	{
		public BlurNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Type = BlurType.Linear;
			Radius = 1;
		}

		[DataMember]
		public BlurType Type { get; set; }

		[DataMember]
		[Range(0.0, double.PositiveInfinity)]
		public int Radius { get; set; }

		#region INode Members

		public override Frame[] Process(Frame[] inputs, int tick)
		{
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

		#endregion

		private Frame LinearBlur(Frame input, int tick)
		{
			// only needs to be calculated once
			float factor = 1F / (2 * Radius + 1);

			/* Since this Array is sort of ugly now: workspace[BlurringDimension, x-coord, y-coord, colorchannel] */
			float[, , ,] workspace = new float[2, input.Size.Width, input.Size.Height, 3];

			/* Blur horinzontal dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					workspace[0, x, y, 0] = workspace[0, x, y, 1] = workspace[0, x, y, 2] = 0F;
					for (int z = x - Radius; z <= x + Radius; z++) {
						Rgb imagePixel = GetCappedPixels(z, y, input);
						workspace[0, x, y, 0] += factor * imagePixel.R;
						workspace[0, x, y, 1] += factor * imagePixel.G;
						workspace[0, x, y, 2] += factor * imagePixel.B;
					}
				}
			}
			/* Blur vertical dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					workspace[1, x, y, 0] = workspace[1, x, y, 1] = workspace[1, x, y, 2] = 0F;
					for (int z = y - Radius; z <= y + Radius; z++) {
						int cappedY = Math.Min(input.Size.Height - 1, Math.Max(0, z));
						workspace[1, x, y, 0] += factor * workspace[0, x, cappedY, 0];
						workspace[1, x, y, 1] += factor * workspace[0, x, cappedY, 1];
						workspace[1, x, y, 2] += factor * workspace[0, x, cappedY, 2];
					}
				}
			}
			/* Convert floatarray to frame */
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					result[x, y] = new Rgb((byte)workspace[1, x, y, 0], (byte)workspace[1, x, y, 1], (byte)workspace[1, x, y, 2]);
				}
			}
			return result;
		}

		private Frame GaussianBlur(Frame input, int tick)
		{
			/* Since this Array is sort of ugly now: workspace[BlurringDimension, x-coord, y-coord, colorchannel] */
			float[, , ,] workspace = new float[2, input.Size.Width, input.Size.Height, 3];

			/* Blur horinzontal dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					workspace[0, x, y, 0] = workspace[0, x, y, 1] = workspace[0, x, y, 2] = 0F;
					for (int z = x - (3 * Radius); z <= x + (3 * Radius); z++) {
						float factor = G(z - x);
						Rgb imagePixel = GetCappedPixels(z, y, input);
						workspace[0, x, y, 0] += factor * imagePixel.R;
						workspace[0, x, y, 1] += factor * imagePixel.G;
						workspace[0, x, y, 2] += factor * imagePixel.B;
					}
				}
			}
			/* Blur vertical dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					workspace[1, x, y, 0] = workspace[1, x, y, 1] = workspace[1, x, y, 2] = 0F;
					for (int z = y - (3 * Radius); z <= y + (3 * Radius); z++) {
						float factor = G(y - z);
						int cappedY = Math.Min(input.Size.Height - 1, Math.Max(0, z));
						workspace[1, x, y, 0] += factor * workspace[0, x, cappedY, 0];
						workspace[1, x, y, 1] += factor * workspace[0, x, cappedY, 1];
						workspace[1, x, y, 2] += factor * workspace[0, x, cappedY, 2];
					}
				}
			}
			/* Convert floatarray to frame */
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					result[x, y] = new Rgb((byte)workspace[1, x, y, 0], (byte)workspace[1, x, y, 1], (byte)workspace[1, x, y, 2]);
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
	}
}
