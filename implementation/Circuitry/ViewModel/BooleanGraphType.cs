using System.ComponentModel;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;

namespace Circuitry.ViewModel
{
	[DisplayName("Boolean")]
	public class BooleanGraphType : IGraphType
	{
		public bool DependsOnReference { get { return false; } }

		public bool DependsOnAnnotatedReference { get { return false; } }

		public bool DependsOnLogfile { get { return false; } }

		public double Process(Frame frame, Frame reference)
		{
			return DigitalNode.FrameToBool(frame) ? 1 : 0;
		}
	}
}
