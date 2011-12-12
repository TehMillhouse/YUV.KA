using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class ReplayStateViewModel
	{
		public int FrameIndex
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public int TickCount { get { throw new NotImplementedException(); } }
		public bool IsPlaying { get { throw new NotImplementedException(); } }
	
		public void Slower()
		{
			throw new System.NotImplementedException();
		}

		public void PlayPause()
		{
			throw new System.NotImplementedException();
		}

		public void Stop()
		{
			throw new System.NotImplementedException();
		}

		public void Faster()
		{
			throw new System.NotImplementedException();
		}
	}
}
