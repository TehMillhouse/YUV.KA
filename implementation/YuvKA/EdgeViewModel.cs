using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace YuvKA
{
	public class EdgeViewModel
	{
		Point Start { get; set; }
		Point End { get; set; }
		Path Path { get; private set; }
	}
}
