namespace YuvKA.VideoModel
{
	public class AnnotatedFrame : Frame
	{
		public AnnotatedFrame(Size size, MacroblockDecision[,] decisions)
			: base(size)
		{
			Decisions = decisions;
		}

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
