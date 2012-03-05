using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This interface provides a gerneral interface
	/// for all types of overlay.
	/// </summary>
	[InheritedExport]
	public interface IOverlayType
	{
		/// <summary>
		/// The value if this type of overlay requires a reference frame
		/// additionally to the regular inputframe
		/// </summary>
		bool DependsOnReference { get; }

		/// <summary>
		/// The value if this type of overlay requires encoderlogdata
		/// </summary>
		bool DependsOnLogfiles { get; }

		/// <summary>
		/// The value if this type of overlay requires movevectordata
		/// </summary>
		bool DependsOnVectors { get; }
	
		/// <summary>
		/// Processes the input according to this type of overlay.
		/// </summary>
		/// <param name="input">An array of Frames as input for processing.</param>
		/// <returns>The result of Overlaying.</returns>
		Frame Process(Frame[] input);
	}
}
