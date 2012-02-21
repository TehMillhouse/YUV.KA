using System.ComponentModel.Composition;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Represents the GraphType of an DiagramGraph.
	/// </summary>
	[InheritedExport]
	public interface IGraphType
	{
		/// <summary>
		/// Gets whether the IGgraphType requires a reference.
		/// </summary>
		bool DependsOnReference { get; }

		/// <summary>
		/// Gets whether the IGgraphType requires an reference
		/// with an attached logfile.
		/// </summary>
		bool DependsOnAnnotatedReference { get; }

		/// <summary>
		/// Gets whether the IGgraphType requires an attached logfile.
		/// </summary>
		bool DependsOnLogfile { get; }

		/// <summary>
		/// Calculates the Datat according to a specific algrithm.
		/// </summary>
		/// <param name="frame">The given frame.</param>
		/// <param name="reference">The frame against which the given frame is compared.</param>
		double Process(Frame frame, Frame reference);
	}
}
