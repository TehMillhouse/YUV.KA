using System;
using YuvKA.VideoModel;
namespace YuvKA.Pipeline.Implementation
{
	public class ArtifactsOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return true; } }
		public bool DependsOnLogfiles { get { return false; } }
		public bool DependsOnVectors { get { return false; } }

		public Frame Process(Frame frame, Frame reference)
		{
			Frame result = new Frame(frame.Size);
			for (int x = 0; x < frame.Size.Width; x++) {
				for (int y = 0; y < frame.Size.Height; y++) {
					int difference = Math.Abs(frame[x, y].R - reference[x, y].R);
					difference += Math.Abs(frame[x, y].G - reference[x, y].G);
					difference += Math.Abs(frame[x, y].B - reference[x, y].B);
					result[x, y] = (difference >= 50) ? new Rgb(255, 0, 0) : frame[x, y];
				}
			}
			return result;
		}
	}

	public class MoveVectorsOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return false; } }
		public bool DependsOnLogfiles { get { return false; } }
		public bool DependsOnVectors { get { return true; } }

		public Frame Process(Frame frame, Frame reference)
		{
			throw new System.NotImplementedException();
		}
	}

	public class BlocksOverlay : IOverlayType
	{
		public bool DependsOnReference { get { return false; } }
		public bool DependsOnLogfiles { get { return true; } }
		public bool DependsOnVectors { get { return false; } }

		public Frame Process(Frame frame, Frame reference)
		{
			AnnotatedFrame frameWithLogs = (AnnotatedFrame)frame;
			Frame result = new Frame(frame.Size);
			for (int x = 0; x < frame.Size.Width; x++) {
				for (int y = 0; y < frame.Size.Height; y++) {
					result[x, y] = GetMaskedPixel(frame[x, y], x + 1, y + 1, frameWithLogs.Decisions[x / 16, y / 16].PartitioningDecision);
				}
			}
			return result;
		}

		private Rgb GetMaskedPixel(Rgb pixel, int x, int y, MacroblockPartitioning decision)
		{
			switch (decision) {
				case MacroblockPartitioning.Inter16x16:
					if ((x % 16 == 0) || (y % 16 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Inter16x8:
					if ((x % 16 == 0) || (y % 8 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Inter4x4:
					if ((x % 4 == 0) || (y % 4 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Inter4x8:
					if ((x % 4 == 0) || (y % 8 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Inter8x16:
					if ((x % 8 == 0) || (y % 16 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Inter8x4:
					if ((x % 8 == 0) || (y % 4 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Inter8x8:
					if ((x % 8 == 0) || (y % 8 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				/* TODO Ask Kobbe what this is */
				case MacroblockPartitioning.Inter8x8OrBelow:
					if ((x % 8 == 0) || (y % 8 == 0))
						return new Rgb(255, 255, 255);
					else
						return pixel;
				case MacroblockPartitioning.Intra16x16:
					if ((x % 16 == 0) || (y % 16 == 0))
						return new Rgb(255, 255, 255);
					else
						return new Rgb((byte)Math.Max(255, pixel.R + 40), pixel.G, pixel.B);
				case MacroblockPartitioning.Intra4x4:
					if ((x % 4 == 0) || (y % 4 == 0))
						return new Rgb(255, 255, 255);
					else
						return new Rgb((byte)Math.Max(255, pixel.R + 40), pixel.G, pixel.B);
				case MacroblockPartitioning.Intra8x8:
					if ((x % 8 == 0) || (y % 8 == 0))
						return new Rgb(255, 255, 255);
					else
						return new Rgb((byte)Math.Max(255, pixel.R + 40), pixel.G, pixel.B);
				default:
					if ((x % 16 == 0) || (y % 16 == 0))
						return new Rgb(255, 255, 255);
					else
						return new Rgb(pixel.R, pixel.G, (byte)Math.Max(255, pixel.B + 40));
			}
		}
	}
}