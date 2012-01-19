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
		[Browsable(false)]
		public IOverlayType Type { get; set; }

		[Browsable(false)]
		public Frame Data { get; private set; }

		public OverlayViewModel Window { get { return new OverlayViewModel(this); } }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			Data = Type.Process(inputs[0], (Type.DependsOnReference) ? inputs[1] : null);
		}
	}
}
