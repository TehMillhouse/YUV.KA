using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public abstract class Node
	{
		public class Input
		{
			public Output Source { get; set; }
		}

		public class Output
		{
			public Frame Buffer { get; set; }

			public Node Node { get; private set; }

			public Output(Node node)
			{
				Node = node;
			}
		}

		public ICollection<Input> Inputs { get; private set; }

		public ICollection<Output> Outputs { get; private set; }
	
		public abstract void ProcessFrame();

		Output AddOutput()
		{
			Output o = new Output(this);
			Outputs.Add(o);
			return o;
		}
	}
}
