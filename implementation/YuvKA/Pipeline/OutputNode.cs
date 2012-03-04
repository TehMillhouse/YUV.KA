using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	[DataContract]
	public abstract class OutputNode : Node
	{
		public OutputNode(int? inputCount)
			: base(inputCount: inputCount, outputCount: 0)
		{
			Name = "Output";
		}

		public sealed override Frame[] Process(Frame[] inputs, int tick)
		{
			ProcessCore(inputs, tick);
			return new Frame[] { };
		}

		public abstract void ProcessCore(Frame[] inputs, int tick);
	}
}
