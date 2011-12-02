using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	public abstract class Node : IDisposable
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

		public double X { get; set; }
		public double Y { get; set; }

		public ICollection<Input> Inputs { get; private set; }

		public ICollection<Output> Outputs { get; private set; }
	
		public abstract void ProcessFrame(int frameIndex);

		Output AddOutput()
		{
			Output o = new Output(this);
			Outputs.Add(o);
			return o;
		}

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion
	}
}
