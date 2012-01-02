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
				result[0] = new Frame(inputs[0].Size);
				for (int x = 0; x < inputs[0].Size.Width; x++) {
					for (int y = 0; y < inputs[0].Size.Height; y++) {
						result[0][x, y] = new Rgb(0, 0, 0);
						for (int xi = x - (3 * Radius); xi <= x + (3 * Radius); xi++) {
							for (int yi = y - (3 * Radius); yi <= y + (3 * Radius); yi++) {
								int newR = (int)(result[0][x, y].R + G(xi - x, yi - y) * GetCappedPixels(xi, yi, inputs[0]).R);
								int newG = (int)(result[0][x, y].G + G(xi - x, yi - y) * GetCappedPixels(xi, yi, inputs[0]).G);
								int newB = (int)(result[0][x, y].B + G(xi - x, yi - y) * GetCappedPixels(xi, yi, inputs[0]).B);
								result[0][x, y] = new Rgb((byte)newR, (byte)newB, (byte)newG);
							}
						}
					}
				}
				inputs[0] = result[0];
			}
			else if (Type == BlurType.Linear) {
				result[0] = new Frame(inputs[0].Size);
				for (int x = 0; x < inputs[0].Size.Width; x++) {
					for (int y = 0; y < inputs[0].Size.Height; y++) {
						result[0][x, y] = new Rgb(0, 0, 0);
						for (int xi = x - Radius; xi <= x + Radius; xi++) {
							for (int yi = y - Radius; yi <= y + Radius; yi++) {
								int newR = result[0][x, y].R + (int)(((double)1 / (Radius * Radius)) * GetCappedPixels(xi, yi, inputs[0]).R);
								int newG = result[0][x, y].G + (int)(((double)1 / (Radius * Radius)) * GetCappedPixels(xi, yi, inputs[0]).G);
								int newB = result[0][x, y].B + (int)(((double)1 / (Radius * Radius)) * GetCappedPixels(xi, yi, inputs[0]).B);
								result[0][x, y] = new Rgb((byte)newR, (byte)newB, (byte)newG);
							}
						}
					}
				}
			}
			return result;
		}

		#endregion

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
