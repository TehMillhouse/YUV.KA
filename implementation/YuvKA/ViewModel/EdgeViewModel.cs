using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace YuvKA.ViewModel
{
	public class EdgeViewModel
	{
        public Point Start { get; set; }
        public Point End { get; set; }
        public Path Path { get; private set; }
	}
}
