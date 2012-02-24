using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuvKA.VideoModel;

namespace YuvKA.Test
{
	/// <summary>
	/// Provides methods to help with generating anonymous nodes and dummy annotated frames for testcases.
	/// </summary>
	static class AnonNodeHelper
	{
		/// <summary>
		/// A Process method to be used by AnonymousNodes. 
		/// Generates an array of 1 Frame and 2 AnnotatedFrame with randomly filled Data.
		/// </summary>
		/// <param name="inputs">The inputs used for processing. This parameter is not used here.</param>
		/// <param name="tick"> The current index of the frame. This parameter is not used here.</param>
		/// <returns> An array of generated Frames.</returns>
		public static Frame[] SourceNode(Frame[] inputs, int tick)
		{
			var testSize = new YuvKA.VideoModel.Size(8, 8);
			Frame[] outputs = { GenerateAnnFrame(new MacroblockDecision[,] 
				{ { new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Inter4x4 },
					new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Inter4x4 },
					new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Inter8x4 } } }),
					new Frame(testSize), 
				GenerateAnnFrame(new MacroblockDecision[,] { {
					new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Intra4x4 },
					new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Intra4x4 },
					new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Inter8x4 } } }),
				};
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					outputs[1][x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
				}
			}
			return outputs;
		}

		/// <summary>
		/// Generates an AnnotatedFrame with randomly filled Data and the given Macroblock decisions.
		/// </summary>
		/// <returns> An annotated Frame with random Data and the given Macroblock decisions.</returns>
		public static AnnotatedFrame GenerateAnnFrame(MacroblockDecision[,] decisions)
		{
			var testSize = new YuvKA.VideoModel.Size(8, 8);
			var output = new AnnotatedFrame(testSize, decisions);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					output[x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			return output;
		}
	}
}
