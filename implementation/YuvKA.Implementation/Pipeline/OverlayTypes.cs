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
			throw new System.NotImplementedException();
		}
	}
}