using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class OverlayNode : OutputNode
	{
		public OverlayNode()
			: base(inputCount: 2)
		{
		}

		[DataMember]
		public IOverlayType Type { get; set; }

		public Frame Data { get; private set; }

		[Browsable(true)]
		public OverlayViewModel Window { get { return new OverlayViewModel(this); } }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			Data = Type.Process(inputs[0], (Type.DependsOnReference) ? inputs[1] : null);
		}
	}
}
