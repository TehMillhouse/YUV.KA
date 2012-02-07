using System.ComponentModel.Composition;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[InheritedExport]
	public interface IGraphType
	{
		bool DependsOnReference { get; }
		bool DependsOnAnnotatedReference { get; }
		bool DependsOnLogfile { get; }

		double Process(Frame frame, Frame reference);
	}
}
