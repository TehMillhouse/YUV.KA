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
			Name = "Overlay";
			Type = new NoOverlay();
		}

		[DataMember]
		public IOverlayType Type { get; set; }

		public Frame Data { get; private set; }

		[Browsable(true)]
		public OverlayViewModel Window { get { return new OverlayViewModel(this); } }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			if (Type.DependsOnReference && inputs.Length < 2)
				SetBlackFrame(inputs[0].Size);
			else if (Type.DependsOnLogfiles && (!(inputs[0] is AnnotatedFrame) || ((AnnotatedFrame)inputs[0]).Decisions[0, 0].PartitioningDecision == null))
				SetBlackFrame(inputs[0].Size);
			else if (Type.DependsOnVectors && (!(inputs[0] is AnnotatedFrame) || ((AnnotatedFrame)inputs[0]).Decisions[0, 0].Movement == null))
				SetBlackFrame(inputs[0].Size);
			else
				Data = Type.Process(inputs);
		}

		private void SetBlackFrame(Size size)
		{
			Frame blackFrame = new Frame(size);
			Rgb black = new Rgb(0, 0, 0);
			for (int x = 0; x < size.Width; x++) {
				for (int y = 0; y < size.Height; y++) {
					blackFrame[x, y] = black;
				}
			}
			Data = blackFrame;
		}
	}
}
