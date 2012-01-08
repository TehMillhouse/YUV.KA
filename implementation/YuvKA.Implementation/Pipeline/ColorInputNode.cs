using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class ColorInputNode : InputNode
	{
		Rgb color;
		Frame outputFrame;
		bool colorChanged;

		public ColorInputNode()
			: base(outputCount: 1)
		{
		}

		[DataMember]
		public Rgb Color
		{
			get { return color; }
			set
			{
				color = value;
				colorChanged = true;
			}
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			outputFrame = null;
			// Have to fill the new frame with color
			colorChanged = true;
		}

		#region INode Members

		public override Frame OutputFrame(int tick)
		{
			EnsureInputLoaded();
			return outputFrame;
		}

		#endregion

		private void EnsureInputLoaded()
		{
			// If the size was changed
			if (outputFrame == null) {
				outputFrame = new Frame(new Size(Size.Width, Size.Height));
			}

			if (colorChanged) {
				// Fill the frame with the selected color
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					for (int y = 0; y < outputFrame.Size.Height; ++y) {
						outputFrame[x, y] = Color;
					}
				}
				colorChanged = false;
			}
		}
	}
}
