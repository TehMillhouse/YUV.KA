using System.Windows;

namespace YuvKA.VideoModel
{
	public class MacroblockDecision
	{
		public MacroblockDecision(Vector movement, MacroblockPartitioning partitioningDecision)
		{
			Movement = movement;
			PartitioningDecision = partitioningDecision;
		}

		public Vector Movement { get; private set; }

		public MacroblockPartitioning PartitioningDecision { get; private set; }
	}
}
