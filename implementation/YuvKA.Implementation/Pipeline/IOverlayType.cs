using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[InheritedExport]
	public interface IOverlayType
	{
		bool DependsOnReference { get; }
		bool DependsOnLogfiles { get; }
		bool DependsOnVectors { get; }
	
		Frame Process(Frame[] input);
	}
}
