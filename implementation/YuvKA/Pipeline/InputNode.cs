using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	/// <summary>
	/// This abstract class represents the structure an input node should have.
	/// </summary>
	[DataContract]
	public abstract class InputNode : Node
	{
		Size size;

		/// <summary>
		/// Constructs an InputNode with a specified number of outputs. Null represents a variable number of outputs.
		/// </summary>
		public InputNode(int? outputCount)
			: base(inputCount: 0, outputCount: outputCount)
		{
			size = new Size(100, 100);
		}

		/// <summary>
		/// Represents the size of a frame.
		/// </summary>
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

		/// <summary>
		/// Represents the number of frames this input node can produce. The value null represents a (theoretically) infinite number of frames.
		/// </summary>
		public virtual int? TickCount { get { return null; } }

		/// <summary>
		/// Creates the frame that belongs to the specified tick, according to the task of the inheriting node.
		/// </summary>
		public sealed override Frame[] Process(Frame[] inputs, int tick)
		{
			return new[] { OutputFrame(tick) };
		}

		/// <summary>
		/// Returns the for the specified tick created frame.
		/// </summary>
		public abstract Frame OutputFrame(int tick);

		protected virtual void OnSizeChanged() { }
	}
}
