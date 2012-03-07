using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class represents the posibility of overlaying
	/// the input with various options, and store the result,
	/// so other classes can fetch it.
	/// </summary>
	[DataContract]
	[Description("This Node can overlay its input to provide visually more data about it")]
	public class OverlayNode : OutputNode
	{
		/// <summary>
		/// Constructs an overlaynode.
		/// It has 2 inputs, and standard overlay is "NoOverlay"
		/// </summary>
		public OverlayNode()
			: base(inputCount: 2)
		{
			Name = "Overlay";
			Type = new NoOverlay();
		}

		/// <summary>
		/// The type of overlay that shall be applied on the input.
		/// </summary>
		[DataMember]
		public IOverlayType Type { get; set; }

		/// <summary>
		/// The result of the last overlayed Frame.
		/// </summary>
		public Frame Data { get; private set; }

		public override bool InputIsValid
		{
			get
			{
				return (Inputs[0].Source != null) && (Inputs[0].Source.Node.InputIsValid);
			}
		}

		/// <summary>
		/// The Viewmodel of the window that shall be displayed
		/// </summary>
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
