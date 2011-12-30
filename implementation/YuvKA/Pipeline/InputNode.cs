using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	[DataContract]
	public abstract class InputNode : Node
	{
		public InputNode(int? outputCount)
			: base(inputCount: 0, outputCount: outputCount)
		{
		}

		[DataMember]
		public Size Size { get; set; }

		[Browsable(false)]
		public virtual int TickCount { get { return 1; } }

		public sealed override Frame[] Process(Frame[] inputs, int tick)
		{
			return new[] { OutputFrame(tick) };
		}

		public abstract Frame OutputFrame(int tick);
	}
}
