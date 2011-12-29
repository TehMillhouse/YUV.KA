
namespace YuvKA.VideoModel
{
	public class AnnotatedFrame : Frame
	{
		public AnnotatedFrame(Size size, MacroblockDecision[] decisions)
			: base(size)
		{
			Decisions = decisions;
		}

		public MacroblockDecision[] Decisions { get; private set; }
	}
}
