using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[InheritedExport]
	public interface IGraphType
	{
		bool DependsOnReference { get; }

		bool DependsOnLogfile { get; }

		double Process(Frame frame, Frame reference);
	}
}
