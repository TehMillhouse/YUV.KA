using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class BlurNode : Node
	{
		private const double PI = 3.14159265358979323846;
		private const double E = 2.718281828459045235360;

		public BlurNode() : base(inputCount: 1, outputCount: 1)
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
			if (Type == BlurType.Gaussian)
			{
				Frame newFrame = new Frame(inputs[0].Size);
				for (int x = 0; x < inputs[0].Size.Width; x++)
				{
					for (int y = 0; y < inputs[0].Size.Height; y++)
					{
						newFrame[x, y] = new Rgb(0, 0, 0);
						for (int xi = x - (3 * Radius); xi <= x + (3 * Radius); xi++)
						{
							for (int yi = y - (3 * Radius); yi <= y + (3 * Radius); yi++)
							{
								int newR = Convert.ToInt32(newFrame[x, y].R + G(xi - x, yi - y) * GetCappedPixels(xi, yi, inputs[0]).R);
								int newG = Convert.ToInt32(newFrame[x, y].G + G(xi - x, yi - y) * GetCappedPixels(xi, yi, inputs[0]).G);
								int newB = Convert.ToInt32(newFrame[x, y].B + G(xi - x, yi - y) * GetCappedPixels(xi, yi, inputs[0]).B);
								newFrame[x, y] = new Rgb((byte)newR, (byte)newB, (byte)newG);
							}
						}
					}
				}
				inputs[0] = newFrame;
			}
			else if (Type == BlurType.Linear)
			{
				Frame newFrame = new Frame(inputs[0].Size);
				for (int x = 0; x < inputs[0].Size.Width; x++)
				{
					for (int y = 0; y < inputs[0].Size.Height; y++)
					{
						newFrame[x, y] = new Rgb(0, 0, 0);
						for (int xi = x - Radius; xi <= x + Radius; xi++)
						{
							for (int yi = y - Radius; yi <= y + Radius; yi++)
							{
								int newR = newFrame[x, y].R + (1 / (Radius * Radius)) * GetCappedPixels(xi, yi, inputs[0]).R;
								int newG = newFrame[x, y].G + (1 / (Radius * Radius)) * GetCappedPixels(xi, yi, inputs[0]).G;
								int newB = newFrame[x, y].B + (1 / (Radius * Radius)) * GetCappedPixels(xi, yi, inputs[0]).B;
								newFrame[x, y] = new Rgb((byte)newR, (byte)newB, (byte)newG);
							}
						}
					}
				}
				inputs[0] = newFrame;
			}
			return inputs;
		}

		#endregion

		private double G(int x, int y)
		{
			return (1 / (2 * PI * Radius * Radius)) * Math.Pow(E, -1 * ((x * x + y * y) / (2 * Radius * Radius)));
		}

		private Rgb GetCappedPixels(int x, int y, Frame frame)
		{
			int cappedX = x;
			int cappedY = y;
			if (cappedX >= frame.Size.Width)
			{
				cappedX = frame.Size.Width - 1;
			}
			else if (cappedX < 0)
			{
				cappedX = 0;
			}
			if (cappedY >= frame.Size.Height)
			{
				cappedY = frame.Size.Height - 1;
			}
			else if (cappedY < 0)
			{
				cappedY = 0;
			}
			return frame[cappedX, cappedY];
		}
	}
}
