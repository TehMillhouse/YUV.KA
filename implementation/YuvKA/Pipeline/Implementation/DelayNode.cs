using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class DelayNode : Node
	{
		Queue<Frame> queue = new Queue<Frame>();

		[Range(0, 10)]
		public int Delay { get; set; }

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}
	}
}
