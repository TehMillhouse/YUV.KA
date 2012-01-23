using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using YuvKA.VideoModel;


namespace YuvKA.Pipeline.Implementation
{
	[DisplayName("No Overlay")]
	public class NoOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return false; } }
		public bool DependsOnLogfiles { get { return false; } }
		public bool DependsOnVectors { get { return false; } }

		public Frame Process(Frame[] input)
		{
			return input[0];
		}
	}

	[DisplayName("Artifact-Overlay")]
	public class ArtifactsOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return true; } }
		public bool DependsOnLogfiles { get { return false; } }
		public bool DependsOnVectors { get { return false; } }

		public Frame Process(Frame[] input)
		{
			Frame result = new Frame(input[0].Size);
			for (int x = 0; x < input[0].Size.Width; x++) {
				for (int y = 0; y < input[0].Size.Height; y++) {
					int difference = Math.Abs(input[0][x, y].R - input[1][x, y].R);
					difference += Math.Abs(input[0][x, y].G - input[1][x, y].G);
					difference += Math.Abs(input[0][x, y].B - input[1][x, y].B);
					result[x, y] = (difference >= 40) ? new Rgb(255, 0, 0) : input[0][x, y];
				}
			}
			return result;
		}
	}

	[DisplayName("Movementvector-Overlay")]
	public class MoveVectorsOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return false; } }
		public bool DependsOnLogfiles { get { return false; } }
		public bool DependsOnVectors { get { return true; } }

		public Frame Process(Frame[] input)
		{
			Frame result = new Frame(input[0]);
			AnnotatedFrame frameWithLogs = (AnnotatedFrame)input[0];
			/* Create Bitmap to draw the vectors on */
			using (Bitmap drawableFrame = new Bitmap(input[0].Size.Width, input[0].Size.Width)) {
				for (int x = 0; x < input[0].Size.Width; x++) {
					for (int y = 0; y < input[0].Size.Height; y++) {
						Rgb pixel = input[0][x, y];
						drawableFrame.SetPixel(x, y, Color.FromArgb(pixel.R, pixel.G, pixel.B));
					}
				}
				/* Draw the movementvector of each macroblock */
				for (int x = 0; x < (input[0].Size.Width / 16); x++) {
					for (int y = 0; y < (input[0].Size.Height / 16); y++) {
						DrawVector(drawableFrame, x * 16, y * 16, GetScaledVector(14.0, frameWithLogs.Decisions[x, y].Movement));
					}
				}
				/* Print the frame with vectors into the resultframe */
				for (int x = 0; x < input[0].Size.Width; x++) {
					for (int y = 0; y < input[0].Size.Height; y++) {
						Rgb pixel = new Rgb(drawableFrame.GetPixel(x, y).R, drawableFrame.GetPixel(x, y).G, drawableFrame.GetPixel(x, y).B);
						result[x, y] = pixel;
					}
				}
			}
			return result;
		}

		private void DrawVector(Bitmap frame, int xOffset, int yOffset, Vector movement)
		{
			/* Draw Arrowshaft */
			Graphics drawableMacroblock = Graphics.FromImage(frame);
			using (Pen newPen = new Pen(Color.White, 1.5F)) {
				float halfXDiff = (float)(0.5 * movement.X);
				float halfYDiff = (float)(0.5 * movement.Y);
				float headX = xOffset + 7 + halfXDiff;
				float headY = yOffset + 7 - halfYDiff;
				drawableMacroblock.DrawLine(newPen, xOffset + 7 - halfXDiff, yOffset + 7 + halfYDiff, headX, headY);
				/* Draw Arrowhead */
				double headLength = Math.Sqrt(movement.X * movement.X + movement.Y * movement.Y) / 3.0;
				double alpha = Math.Atan(movement.Y / Math.Abs(movement.X));
				double alphaH1 = alpha + (Math.PI / 4);
				float headX1 = (float)(headX - Math.Cos(alphaH1) * headLength * ((movement.X < 0) ? -1 : 1));
				float headY1 = (float)(headY + Math.Sin(alphaH1) * headLength);
				drawableMacroblock.DrawLine(newPen, headX1, headY1, headX, headY);
				double alphaH2 = alpha - (Math.PI / 4);
				float headX2 = (float)(headX - Math.Cos(alphaH2) * headLength * ((movement.X < 0) ? -1 : 1));
				float headY2 = (float)(headY + Math.Sin(alphaH2) * headLength);
				drawableMacroblock.DrawLine(newPen, headX2, headY2, headX, headY);
			}
		}

		private Vector GetScaledVector(double cap, Vector unscaledVector)
		{
			double scaleFactor = Math.Max(Math.Max(Math.Abs(unscaledVector.X), Math.Abs(unscaledVector.Y)), cap) / cap;
			return new Vector(unscaledVector.X / scaleFactor, unscaledVector.Y / scaleFactor);
		}
	}

	[DisplayName("Macroblock-Overlay")]
	public class BlocksOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return false; } }
		public bool DependsOnLogfiles { get { return true; } }
		public bool DependsOnVectors { get { return false; } }

		public Frame Process(Frame[] input)
		{
			AnnotatedFrame frameWithLogs = (AnnotatedFrame)input[0];
			Frame result = new Frame(input[0].Size);
			for (int x = 0; x < input[0].Size.Width; x++) {
				for (int y = 0; y < input[0].Size.Height; y++) {
					result[x, y] = GetMaskedPixel(input[0][x, y], x + 1, y + 1, frameWithLogs.Decisions[x / 16, y / 16].PartitioningDecision);
				}
			}
			return result;
		}

		private Rgb GetMaskedPixel(Rgb pixel, int x, int y, MacroblockPartitioning? decision)
		{
			switch (decision) {
				case MacroblockPartitioning.Inter16x16:
					if ((x % 16 == 0) || (y % 16 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Inter16x8:
					if ((x % 16 == 0) || (y % 8 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Inter4x4:
					if ((x % 4 == 0) || (y % 4 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Inter4x8:
					if ((x % 4 == 0) || (y % 8 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Inter8x16:
					if ((x % 8 == 0) || (y % 16 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Inter8x4:
					if ((x % 8 == 0) || (y % 4 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Inter8x8:
					if ((x % 8 == 0) || (y % 8 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				/* TODO Ask Kobbe what this is */
				case MacroblockPartitioning.Inter8x8OrBelow:
					if ((x % 8 == 0) || (y % 8 == 0))
						return new Rgb(0, 0, 0);
					else
						return pixel;
				case MacroblockPartitioning.Intra16x16:
					if ((x % 16 == 0) || (y % 16 == 0))
						return new Rgb(0, 0, 0);
					else
						return new Rgb((byte)Math.Max(255, pixel.R + 40), pixel.G, pixel.B);
				case MacroblockPartitioning.Intra4x4:
					if ((x % 4 == 0) || (y % 4 == 0))
						return new Rgb(0, 0, 0);
					else
						return new Rgb((byte)Math.Max(255, pixel.R + 40), pixel.G, pixel.B);
				case MacroblockPartitioning.Intra8x8:
					if ((x % 8 == 0) || (y % 8 == 0))
						return new Rgb(0, 0, 0);
					else
						return new Rgb((byte)Math.Max(255, pixel.R + 40), pixel.G, pixel.B);
				default:
					if ((x % 16 == 0) || (y % 16 == 0))
						return new Rgb(0, 0, 0);
					else
						return new Rgb(pixel.R, pixel.G, (byte)Math.Max(255, pixel.B + 40));
			}
		}
	}
}