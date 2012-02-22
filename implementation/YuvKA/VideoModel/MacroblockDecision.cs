using System.Windows;

namespace YuvKA.VideoModel
{
	/// <summary>
	/// A class representing the encoder decision per macroblock (16x16 image block)
	/// </summary>
	public class MacroblockDecision
	{
		/// <summary>
		/// A vector that specifies where the sample data of this macroblock is taken from in the previous frame.
		/// </summary>
		public Vector Movement { get; set; }

		/// <summary>
		/// The decision concerning partitioning that the encoder took at this macroblock.
		/// </summary>
		public MacroblockPartitioning? PartitioningDecision { get; set; }
	}
}
