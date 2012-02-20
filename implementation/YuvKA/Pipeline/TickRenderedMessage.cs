using System.Collections.Generic;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	/// <summary>
	/// Holds the result of rendering a tick with a PipelineDriver: a rendered frame per displayed node output.
	/// </summary>
	public class TickRenderedMessage
	{
		IDictionary<Node.Output, Frame> dic;

		public TickRenderedMessage(IDictionary<Node.Output, Frame> dic)
		{
			this.dic = dic;
		}

		/// <summary>
		/// Gets the frame rendered for the given output or null if the output hasn't been rendered.
		/// </summary>
		public Frame this[Node.Output output]
		{
			get { return dic.ContainsKey(output) ? dic[output] : null; }
		}
	}
}
