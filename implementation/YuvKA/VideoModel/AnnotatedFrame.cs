namespace YuvKA.VideoModel
{
	public class AnnotatedFrame : Frame
	{
		public AnnotatedFrame(Size size, MacroblockDecision[,] decisions)
			: base(size)
		{
			Decisions = decisions;
		}

		public AnnotatedFrame(Frame frame, MacroblockDecision[,] decisions)
			: base(frame.Size)
		{
			
		}

		public MacroblockDecision[,] Decisions { get; private set; }
	}
}
