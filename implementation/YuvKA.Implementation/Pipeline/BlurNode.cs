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
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					result[x, y] = new Rgb(0, 0, 0);
					for (int xi = x - Radius; xi <= x + Radius; xi++) {
						for (int yi = y - Radius; yi <= y + Radius; yi++) {
							int newR = result[x, y].R + (int)(((double)1 / (Radius * Radius)) * GetCappedPixels(xi, yi, input).R);
							int newG = result[x, y].G + (int)(((double)1 / (Radius * Radius)) * GetCappedPixels(xi, yi, input).G);
							int newB = result[x, y].B + (int)(((double)1 / (Radius * Radius)) * GetCappedPixels(xi, yi, input).B);
							result[x, y] = new Rgb((byte)newR, (byte)newG, (byte)newB);
						}
					}
				}
			}
			return result;
		}

		private Frame GaussianBlur(Frame input, int tick)
		{
			Frame result = new Frame(input.Size);
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					result[x, y] = new Rgb(0, 0, 0);
					for (int xi = x - (3 * Radius); xi <= x + (3 * Radius); xi++) {
						for (int yi = y - (3 * Radius); yi <= y + (3 * Radius); yi++) {
							int newR = (int)(result[x, y].R + G(xi - x, yi - y) * GetCappedPixels(xi, yi, input).R);
							int newG = (int)(result[x, y].G + G(xi - x, yi - y) * GetCappedPixels(xi, yi, input).G);
							int newB = (int)(result[x, y].B + G(xi - x, yi - y) * GetCappedPixels(xi, yi, input).B);
							result[x, y] = new Rgb((byte)newR, (byte)newG, (byte)newB);
						}
					}
				}
			}
			return result;
		}

		private double G(int x, int y)
		{
			return (1 / (2 * Math.PI * Radius * Radius)) * Math.Pow(Math.E, -1 * (((double) x * x + y * y) / (2 * Radius * Radius)));
		}

		private Rgb GetCappedPixels(int x, int y, Frame frame)
		{
			int cappedX = Math.Min(frame.Size.Width - 1, Math.Max(0, x));
			int cappedY = Math.Min(frame.Size.Height - 1, Math.Max(0, y));
			return frame[cappedX, cappedY];
		}
	}
}
