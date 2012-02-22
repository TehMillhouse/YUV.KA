namespace YuvKA.VideoModel
{
	/// <summary>
	/// The AnnotatedFrame class is an extension of the Frame class which also includes metadata.
	/// </summary>
	public class AnnotatedFrame : Frame
	{
		/// <summary>
		/// Ctor taking the supposed frame size and the macroblock decisions per frame as parameters
		/// </summary>
		public AnnotatedFrame(Size size, MacroblockDecision[,] decisions)
			: base(size)
		{
			Decisions = decisions;
		}

		/// <summary>
		/// Copy ctor taking the frame to use and the macroblock decisions per frame as parameters
		/// </summary>
		public AnnotatedFrame(Frame frame, MacroblockDecision[] decisions)
			: base(frame)
		{
			Decisions = new MacroblockDecision[frame.Size.Width / 16, frame.Size.Height / 16];
			for (int i = 0; i < decisions.Length; i++ )
			{
				Decisions[i % (frame.Size.Width / 16), i / (frame.Size.Width / 16)] = decisions[i];
			}
		}

		public MacroblockDecision[,] Decisions { get; private set; }
	}
}
