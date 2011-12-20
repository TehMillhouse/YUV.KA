using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public abstract class InputNode : Node
	{
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
