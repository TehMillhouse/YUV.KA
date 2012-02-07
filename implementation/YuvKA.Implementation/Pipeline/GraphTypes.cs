using System;
using System.ComponentModel;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DisplayName("Intra-Block-Frequency")]
	public class IntraBlockFrequency : IGraphType
	{
		public IntraBlockFrequency()
		{
			DependsOnReference = false;
			DependsOnLogfile = true;
			DependsOnAnnotatedReference = false;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }

		public double Process(Frame frame, Frame reference)
		{
			double intraBlocks = 0.0;
			if (frame is AnnotatedFrame) {
				AnnotatedFrame annFrame = (AnnotatedFrame)frame;
				foreach (MacroblockDecision d in annFrame.Decisions) {
					if (d.PartitioningDecision == MacroblockPartitioning.Intra16x16 | d.PartitioningDecision == MacroblockPartitioning.Intra4x4 | d.PartitioningDecision == MacroblockPartitioning.Intra8x8 | d.PartitioningDecision == MacroblockPartitioning.IntraPCM) {
						intraBlocks++;
					}
				}
			}
			return intraBlocks;
		}
	}
	[DisplayName("Peak-Signal-to-Noise-Ratio")]
	public class PeakSignalNoiseRatio : IGraphType
	{
		public PeakSignalNoiseRatio()
		{
			DependsOnReference = true;
			DependsOnLogfile = false;
			DependsOnAnnotatedReference = false;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }

		public double Process(Frame frame, Frame reference)
		{
			double mse = 0.0;
			for (int i = 0; i < frame.Size.Width; i++) {
				for (int j = 0; j < frame.Size.Height; j++) {
					mse += Math.Pow(((frame[i, j].R + frame[i, j].G + frame[i, j].B) - (reference[i, j].R + reference[i, j].G + reference[i, j].B)), 2);
				}
			}
			mse *= (double)1 / (3 * frame.Size.Height * frame.Size.Width);
			if (mse == 0.0)
				return 0.0;
			double psnr = 10 * Math.Log10((Math.Pow((Math.Pow(2, 24) - 1), 2)) / mse);
			return psnr;
		}
	}
	[DisplayName("Pixel-Difference")]
	public class PixelDiff : IGraphType
	{
		public PixelDiff()
		{
			DependsOnReference = true;
			DependsOnLogfile = false;
			DependsOnAnnotatedReference = false;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }
		public double Process(Frame frame, Frame reference)
		{
			double difference = 0.0;
			for (int x = 0; x < frame.Size.Width; x++) {
				for (int y = 0; y < frame.Size.Height; y++) {
					difference += Math.Abs(frame[x, y].R - reference[x, y].R) + Math.Abs(frame[x, y].G - reference[x, y].G) +
						Math.Abs(frame[x, y].B - reference[x, y].B);
				}
			}
			return difference / (3 * frame.Size.Height * frame.Size.Width);
		}
	}
	[DisplayName("Encoderdecision-Difference")]
	public class DecisionDiff : IGraphType
	{
		public DecisionDiff()
		{
			DependsOnReference = true;
			DependsOnLogfile = true;
			DependsOnAnnotatedReference = true;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }

		public double Process(Frame frame, Frame reference)
		{
			double difference = 0.0;
			if (!frame.Size.Equals(reference.Size))
				return difference;
			if (frame is AnnotatedFrame && reference is AnnotatedFrame) {
				AnnotatedFrame annFrame = (AnnotatedFrame)frame;
				AnnotatedFrame annRef = (AnnotatedFrame)reference;
				for (int i = 0; i < annFrame.Decisions.GetLength(0); i++) {
					for (int j = 0; j < annFrame.Decisions.GetLength(1); j++) {
						if (annFrame.Decisions[i, j].Equals(annRef.Decisions[i, j]))
							difference++;
					}
				}
				difference /= annFrame.Decisions.Length;
			}
			return difference;
		}
	}
}
