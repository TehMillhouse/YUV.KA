using System;
using System.ComponentModel;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Provides an IGraphType calculating the Data by counting the number of Intra-Blocks.
	/// </summary>
	[DisplayName("Intra-Block-Frequency")]
	public class IntraBlockFrequency : IGraphType
	{
		/// <summary>
		/// Creates a new IntraBlockFrequency IGraphtype.
		/// This IGraphType requires an attached logfile.
		/// </summary>
		public IntraBlockFrequency()
		{
			DependsOnReference = false;
			DependsOnLogfile = true;
			DependsOnAnnotatedReference = false;
		}


		public bool DependsOnReference { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }

		/// <summary>
		/// Calculates the Data by counting the number of IntraBlocks in the given frame.
		/// </summary>
		/// <param name="reference">The frame against which the given frame is compared. This parameter is unnused.</param>
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

	/// <summary>
	/// Providing an IGraphType calculating the Data by computing the Peak-Signal-to-Noise-Ratio.
	/// </summary>
	[DisplayName("Peak-Signal-to-Noise-Ratio")]
	public class PeakSignalNoiseRatio : IGraphType
	{
		/// <summary>
		/// Creates a new PeakSignalNoiseRatio IGraphtype.
		/// This IGraphType requires a reference frame.
		/// </summary>
		public PeakSignalNoiseRatio()
		{
			DependsOnReference = true;
			DependsOnLogfile = false;
			DependsOnAnnotatedReference = false;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }

		/// <summary>
		/// Calculates the Data by computing the Peak-Signal-to-Noise-Ratio.
		/// </summary>
		/// <param name="reference">The frame against which the given frame is compared</param>
		public double Process(Frame frame, Frame reference)
		{
			if (reference == null)
				return 0;

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

	/// <summary>
	/// Providing an IGraphType calculating the Data by computing color difference between two frames pixel-by-pixel.
	/// </summary>
	[DisplayName("Pixel-Difference")]
	public class PixelDiff : IGraphType
	{
		/// <summary>
		/// Creates a new PixelDiff IGraphtype.
		/// This IGraphType requires a reference frame.
		/// </summary>
		public PixelDiff()
		{
			DependsOnReference = true;
			DependsOnLogfile = false;
			DependsOnAnnotatedReference = false;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }

		/// <summary>
		/// Calculates the Data by computing color difference between two frames pixel-by-pixel.
		/// </summary>
		/// <param name="reference">The frame against which the given frame is compared</param>
		public double Process(Frame frame, Frame reference)
		{
			if (reference == null)
				return 0;

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

	/// <summary>
	/// Providing an IGraphType calculating the Data by computing the number
	/// of similar Encoderdecisions between two frames.
	/// </summary>
	[DisplayName("Encoderdecision-Difference")]
	public class DecisionDiff : IGraphType
	{
		/// <summary>
		/// Creates a new DecisionDiff IGraphtype.
		/// This IGraphType requires a reference frame with an attached 
		/// logfile and a logfile to the given frame
		/// </summary>
		public DecisionDiff()
		{
			DependsOnReference = true;
			DependsOnLogfile = true;
			DependsOnAnnotatedReference = true;
		}

		public bool DependsOnReference { get; private set; }
		public bool DependsOnLogfile { get; private set; }
		public bool DependsOnAnnotatedReference { get; private set; }

		/// <summary>
		/// Calculates the Data by computing the number
		/// of similar Encoderdecisions between two frames.
		/// </summary>
		/// <param name="reference">The frame against which the given frame is compared</param>
		public double Process(Frame frame, Frame reference)
		{
			double difference = 0.0;
			if (frame is AnnotatedFrame && reference is AnnotatedFrame && frame.Size.Height == reference.Size.Height
				&& frame.Size.Width == reference.Size.Width) {
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
