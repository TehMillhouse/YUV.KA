using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	[DataContract]
	public abstract class InputNode : Node
	{
		Size size;

		public InputNode(int? outputCount)
			: base(inputCount: 0, outputCount: outputCount)
		{
			size = new Size(100, 100);
		}

		[DataMember]
		[Browsable(true)]
		public Size Size
		{
			get { return size; }
			set
			{
				size = value;
				OnSizeChanged();
			}
		}

		public virtual int TickCount { get { return 1; } }

		public sealed override Frame[] Process(Frame[] inputs, int tick)
		{
			return new[] { OutputFrame(tick) };
		}

		public abstract Frame OutputFrame(int tick);

		protected virtual void OnSizeChanged() { }
	}
}
