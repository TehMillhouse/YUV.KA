using System;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class IntraBlockFrequency : IGraphType
	{
		public bool DependsOnReference
		{
			get { throw new NotImplementedException(); }
		}

		public double Process(Frame frame, Frame reference)
		{
			throw new NotImplementedException();
		}
	}

	public class PeakSignalNoiseRatio : IGraphType
	{
		public bool DependsOnReference
		{
			get { throw new NotImplementedException(); }
		}

		public double Process(Frame frame, Frame reference)
		{
			throw new NotImplementedException();
		}
	}

	public class PixelDiff : IGraphType
	{
		public bool DependsOnReference
		{
			get { throw new NotImplementedException(); }
		}

		public double Process(Frame frame, Frame reference)
		{
			throw new NotImplementedException();
		}
	}
}
