using System.Collections.Generic;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	public class TickRenderedMessage
	{
		IDictionary<Node.Output, Frame> dic;

		public TickRenderedMessage(IDictionary<Node.Output, Frame> dic)
		{
			this.dic = dic;
		}

		public Frame this[Node.Output output] { get { return dic[output]; } }
	}
}
