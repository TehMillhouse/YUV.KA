using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Implements the possibility to delay a Frame.
	/// </summary>
	[DataContract]
	public class DisplayNode : Node
	{

		/// <summary>
		/// Constructs a DisplayNode.
		/// It has one In- and no Outputs.
		/// </summary>
		public DisplayNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Name = "Display";
		}

		[Browsable(true)]
		public VideoOutputViewModel outputViewModel { get { return new VideoOutputViewModel(this.Outputs[0]); } }

		/// <summary>
		/// Displays the input.
		/// </summary>
		/// <param name="inputs">An array of Frames, with only the first entry regarded.</param>
		/// <param name="tick">The index of the Frame which is processed now.</param>
		/// <returns>The Frame it was given.</returns>
		public override Frame[] Process(Frame[] inputs, int tick)
		{
			return inputs;
		}
	}
}
