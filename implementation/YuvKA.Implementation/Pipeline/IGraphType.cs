using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public interface IGraphType
	{
		bool DependsOnReference
		{
			get;
		}
	
		double Process(Frame frame, Frame reference);
	}
}
