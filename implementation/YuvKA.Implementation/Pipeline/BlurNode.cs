using System;
using System.ComponentModel;
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
			Name = "Blur";
			Type = BlurType.Linear;
			Radius = 1;
		}

		[DataMember]
		[Browsable(true)]
		public BlurType Type { get; set; }

		[DataMember]
		[Range(0.0, 42.0)]
		[Browsable(true)]
		public int Radius { get; set; }

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			if (Radius == 0)
				return inputs;
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
			// only needs to be calculated once
			float factor = 1F / (2 * Radius + 1);

			/* Since this Arrays are sort of ugly now: Arrayname[x-coord, y-coord, colorchannel] */
			float[,,] horizontalBlur = new float[input.Size.Width, input.Size.Height, 3];
			float[,,] verticalBlur = new float[input.Size.Width, input.Size.Height, 3];

			/* Blur horinzontal dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					horizontalBlur[x, y, 0] = horizontalBlur[x, y, 1] = horizontalBlur[x, y, 2] = 0F;
					for (int z = x - Radius; z <= x + Radius; z++) {
						Rgb imagePixel = GetCappedPixels(z, y, input);
						horizontalBlur[x, y, 0] += factor * imagePixel.R;
						horizontalBlur[x, y, 1] += factor * imagePixel.G;
						horizontalBlur[x, y, 2] += factor * imagePixel.B;
					}
				}
			}
			/* Blur vertical dimension */

			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					verticalBlur[x, y, 0] = verticalBlur[x, y, 1] = verticalBlur[x, y, 2] = 0F;
					for (int z = y - Radius; z <= y + Radius; z++) {
						int cappedY = Math.Min(input.Size.Height - 1, Math.Max(0, z));
						verticalBlur[x, y, 0] += factor * horizontalBlur[x, cappedY, 0];
						verticalBlur[x, y, 1] += factor * horizontalBlur[x, cappedY, 1];
						verticalBlur[x, y, 2] += factor * horizontalBlur[x, cappedY, 2];
					}
				}
			}
			/* Convert floatarray to frame */
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
			/* Since this Arrays are sort of ugly now: Arrayname[x-coord, y-coord, colorchannel] */
			float[,,] horizontalBlur = new float[input.Size.Width, input.Size.Height, 3];
			float[,,] verticalBlur = new float[input.Size.Width, input.Size.Height, 3];

			/* Blur horinzontal dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					horizontalBlur[x, y, 0] = horizontalBlur[x, y, 1] = horizontalBlur[x, y, 2] = 0F;
					for (int z = x - (3 * Radius); z <= x + (3 * Radius); z++) {
						float factor = G(z - x);
						Rgb imagePixel = GetCappedPixels(z, y, input);
						horizontalBlur[x, y, 0] += factor * imagePixel.R;
						horizontalBlur[x, y, 1] += factor * imagePixel.G;
						horizontalBlur[x, y, 2] += factor * imagePixel.B;
					}
				}
			}
			/* Blur vertical dimension */
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					verticalBlur[x, y, 0] = verticalBlur[x, y, 1] = verticalBlur[x, y, 2] = 0F;
					for (int z = y - (3 * Radius); z <= y + (3 * Radius); z++) {
						float factor = G(y - z);
						int cappedY = Math.Min(input.Size.Height - 1, Math.Max(0, z));
						verticalBlur[x, y, 0] += factor * horizontalBlur[x, cappedY, 0];
						verticalBlur[x, y, 1] += factor * horizontalBlur[x, cappedY, 1];
						verticalBlur[x, y, 2] += factor * horizontalBlur[x, cappedY, 2];
					}
				}
			}
			/* Convert floatarray to frame */
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					/* Compensate the loss due to disregarding the pixels from 3 * Radius to infinity (for -x, -y, +x, +y) */
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
	}
}
