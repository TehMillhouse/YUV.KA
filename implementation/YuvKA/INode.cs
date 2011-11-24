using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public interface INode
	{
	
		Frame[] Process(YuvKA.Frame[] input);
	}
}
