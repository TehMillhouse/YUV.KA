using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This node simply displays its input. Though every node has that functionality already,
	/// this is duplicated here for usability reasons
	/// </summary>
	[DataContract]
	public class DisplayNode : OutputNode
	{
		/// <summary>
		/// Constructs a DisplayNode
		/// </summary>
		public DisplayNode()
			: base(inputCount: 1)
		{
			Name = "Display";
		}

		public Frame Data { get; private set; }

		/// <summary>
		/// The Viewmodel of the window that shall be displayed
		/// </summary>
		[Browsable(true)]
		public DisplayViewModel Window { get { return new DisplayViewModel(this); } }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			Data = inputs[0];
		}
	}
}