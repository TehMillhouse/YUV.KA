using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
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
