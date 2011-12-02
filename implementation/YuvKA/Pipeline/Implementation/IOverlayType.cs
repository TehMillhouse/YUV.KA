using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public interface IOverlayType
	{
		bool DependsOnReference
		{
			get;
		}
	
		Frame Process(Frame frame, Frame reference);
	}
}
