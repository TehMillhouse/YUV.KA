using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Runtime.Serialization;
using YuvKA.Implementation;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Enables the modification of brightness, contrast and saturation of an input frame.
	/// </summary>
	[DataContract]
	[Description("this Node changes brightness, contrast, and/or saturation of the input")]
	public class BrightnessContrastSaturationNode : Node
	{
		/// <summary>
		/// Creates a new BCS Node with the default values for each property. These values
		/// leave a frame unmodified.
		/// </summary>
		public BrightnessContrastSaturationNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Name = "Brightness/Contrast/Saturation";
			Brightness = 0;
			Contrast = 0;
			Saturation = 0;
		}

		/// <summary>
		/// Gets or sets the brightness value of the frame.
		/// </summary>
		[DataMember]
		[Range(-1.0, 1.0)]
		[Browsable(true)]
		public double Brightness { get; set; }

		/// <summary>
		/// Gets or sets the contrast value of the frame.
		/// </summary>
		[DataMember]
		[Range(-1.0, 1.0)]
		[Browsable(true)]
		public double Contrast { get; set; }

		/// <summary>
		/// Gets or sets the saturation value of the frame.
		/// </summary>
		[DataMember]
		[Range(-1.0, 1.0)]
		[Browsable(true)]
		public double Saturation { get; set; }

		/// <summary>
		/// Produces a new Frame array with one entry, which contains a new, modified copy of the original Frame.
		/// 
		/// The modifications include brightness, contrast and saturation manipulation.
		/// </summary>
		/// <param name="inputs">An array of size one, which contains the input frame</param>
		/// <param name="tick">The index of the current frame in the video stream</param>
		/// <returns>A frame array of size one, which contains the altered frame according to the BCS properties</returns>
		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Frame[] outputFrame = { new Frame(inputs[0].Size) };

			// Shift contrast to work with given [-1, 1] interval
			//double contrast = Contrast + 1.0;

			for (int y = 0; y < inputs[0].Size.Height; ++y) {
				for (int x = 0; x < inputs[0].Size.Width; ++x) {
					Color pixel = Color.FromArgb(inputs[0][x, y].R, inputs[0][x, y].G, inputs[0][x, y].B);
					if (Saturation != 0) {
						// Apply Saturation
						// For the pixel, get its HSL values, modify the Saturation and convert it back to RGB
						double newSaturation = (double)pixel.GetSaturation();
						newSaturation += Saturation * newSaturation;
						pixel = HslHelper.HslToRgb((double)pixel.GetHue(), newSaturation, (double)pixel.GetBrightness());
					}

					int red = pixel.R;
					int green = pixel.G;
					int blue = pixel.B;

					if (Contrast != 0) {
						// Apply Contrast
						double contrast;
						if (Contrast <= 0) {
							contrast = Contrast + 1;
						}
						else {
							contrast = Contrast * 2 + 1;
						}
						contrast *= contrast;

						red = (int)(((((double)red) / 255.0 - 0.5) * contrast + 0.5) * 255.0);
						green = (int)(((((double)green) / 255.0 - 0.5) * contrast + 0.5) * 255.0);
						blue = (int)(((((double)blue) / 255.0 - 0.5) * contrast + 0.5) * 255.0);
					}

					if (Brightness != 0) {
						// Apply Brightness
						red += (int)(127 * Brightness);
						green += (int)(127 * Brightness);
						blue += (int)(127 * Brightness);

						red = (red > 255 ? 255 : (red < 0 ? 0 : red));
						green = (green > 255 ? 255 : (green < 0 ? 0 : green));
						blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
					}

					red = (red > 255 ? 255 : (red < 0 ? 0 : red));
					green = (green > 255 ? 255 : (green < 0 ? 0 : green));
					blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));

					outputFrame[0][x, y] = new Rgb((byte)red, (byte)green, (byte)blue);
				}
			}
			return outputFrame;
		}
	}
}
