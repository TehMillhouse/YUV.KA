using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
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
