using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class EdgeViewModel
	{
		public Node.Output Start { get; set; }
		public Node.Input End { get; set; }
		public Path Path { get { throw new NotImplementedException(); } }
	}
}
