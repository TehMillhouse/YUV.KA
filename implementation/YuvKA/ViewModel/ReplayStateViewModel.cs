using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class ReplayStateViewModel
	{
		public bool IsPlaying { get { throw new NotImplementedException(); } }
		public MainViewModel Parent { get { throw new NotImplementedException(); } }
	
		public void Slower()
		{
			throw new System.NotImplementedException();
		}

		public void PlayPause()
		{
			Parent.Model.Start(Parent.OpenWindows.Select(w => w.NodeModel));
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
