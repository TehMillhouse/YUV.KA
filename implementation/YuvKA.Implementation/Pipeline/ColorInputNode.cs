using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.Windows.Media;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Provides a video stream consisting of a single constant color.
	/// </summary>
	[DataContract]
	public class ColorInputNode : InputNode
	{
		Color color = Color.FromRgb(0,0,0);
		Frame outputFrame;
		bool colorChanged;

		/// <summary>
		/// Creates a new color input node with the default color (black).
		/// </summary>
		public ColorInputNode()
			: base(outputCount: 1)
		{
			Name = "Color";
		}

		/// <summary>
		/// Gets or sets the color of the input frame
		/// </summary>
		[DataMember]
		[Browsable(true)]
		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				colorChanged = true;
			}
		}

		/// <summary>
		/// Ignores the given parameter and returns a frame array of size 1
		/// containing a frame of the specified constant color.
		/// </summary>
		/// <param name="tick">The index of the current frame in the video stream. 
		/// This parameter is unused.</param>
		/// <returns>A frame of the specified constant color</returns>
		public override Frame OutputFrame(int tick)
		{
			EnsureInputLoaded();
			return outputFrame;
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			outputFrame = null;
			// Have to fill the new frame with color
			colorChanged = true;
		}

		private void EnsureInputLoaded()
		{
			// If the size was changed
			if (outputFrame == null) {
				outputFrame = new Frame(new Size(Size.Width, Size.Height));
			}

			if (colorChanged) {
				// Fill the frame with the selected color
				for (int x = 0; x < outputFrame.Size.Width; x++) {
					for (int y = 0; y < outputFrame.Size.Height; y++) {
						outputFrame[x, y] = new Rgb(Color.R, Color.G, Color.B);
					}
				}
				colorChanged = false;
			}
		}
	}
}
