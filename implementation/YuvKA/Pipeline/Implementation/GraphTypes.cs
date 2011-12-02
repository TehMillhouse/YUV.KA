using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class IntraBlockFrequency : IGraphType
	{
		#region IGraphType Members

		public double Process(Frame frame, Frame reference)
		{
			throw new NotImplementedException();
		}

		public bool DependsOnReference
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	public class PixelDiff : IGraphType
	{
		#region IGraphType Members

		public double Process(Frame frame, Frame reference)
		{
			throw new NotImplementedException();
		}

		public bool DependsOnReference
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
