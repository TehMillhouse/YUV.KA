using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	[Description("Changes brightness, contrast, and/or saturation of the input")]
	public class BrightnessContrastSaturationNode : Node
	{
		/// <summary>
		/// Creates a new BCS Node with the default values for each property. These values
		/// leave a frame unmodified.
		/// </summary>
		public BrightnessContrastSaturationNode()
			: base(inputCount: 1, outputCount: 1)
		{
			Brightness = 0;
			Contrast = 0;
			Saturation = 0;
		}

		/// <summary>
		/// The brightness this frame will be set to
		/// </summary>
		[DataMember]
		[Range(-1.0, 1.0)]
		public double Brightness { get; set; }

		/// <summary>
		/// The contrast this frame will be set to
		/// </summary>
		[DataMember]
		[Range(-1.0, 1.0)]
		public double Contrast { get; set; }

		/// <summary>
		/// The saturation this frame will be set to
		/// </summary>
		[DataMember]
		[Range(-1.0, 1.0)]
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
			Frame[] outputFrame = new Frame[1];
			outputFrame[0] = new Frame(inputs[0].Size);
			int red, green, blue;
			double newSaturation;
			Color pixel;
			// Shift contrast to work with given [-1, 1] interval
			double contrast = Contrast + 1.0;

			for (int y = 0; y < inputs[0].Size.Height; ++y) {
				for (int x = 0; x < inputs[0].Size.Width; ++x) {
					pixel = Color.FromArgb(inputs[0][x, y].R, inputs[0][x, y].G, inputs[0][x, y].B);

					// Apply Saturation
					// For the pixel, get its HSL values, modify the Saturation and convert it back to RGB
					newSaturation = (double)pixel.GetSaturation();
					newSaturation += Saturation * newSaturation;
					pixel = HSL_to_RGB((double)pixel.GetHue(), newSaturation, (double)pixel.GetBrightness());

					// Apply Brightness
					red = pixel.R;
					green = pixel.G;
					blue = pixel.B;

					red += (int)(255 * Brightness);
					green += (int)(255 * Brightness);
					blue += (int)(255 * Brightness);

					red = (red > 255 ? 255 : (red < 0 ? 0 : red));
					green = (green > 255 ? 255 : (green < 0 ? 0 : green));
					blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));

					// Apply Contrast
					red = (int)((red - 127) * contrast) + 127;
					green = (int)((green - 127) * contrast) + 127;
					blue = (int)((blue - 127) * contrast) + 127;

					red = (red > 255 ? 255 : (red < 0 ? 0 : red));
					green = (green > 255 ? 255 : (green < 0 ? 0 : green));
					blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));

					outputFrame[0][x, y] = new Rgb((byte)red, (byte)blue, (byte)green);
				}
			}
			return outputFrame;
		}

		/// <summary>
		/// Convertes a color from the HSL to the RGB color space.
		/// </summary>
		/// <param name="h">The hue of the HSL color</param>
		/// <param name="s">The saturation of the HSL color</param>
		/// <param name="l">The lightness of the HSL color</param>
		/// <returns>A RGB Color object transformed from the given HSL color space</returns>
		private Color HSL_to_RGB(double h, double s, double l)
		{
			h /= 360;

			double r = 0, g = 0, b = 0;

			if (l == 0)
				return Color.FromName("black");

			if (s == 0) {
				r = g = b = l;
			}
			else {
				double temp2 = l <= 0.5 ? l * (1.0 + s) : l + s - (l * s);
				double temp1 = 2.0 * l - temp2;

				double[] t3 = new double[] { h + 1.0 / 3.0, h, h - 1.0 / 3.0 };
				double[] clr = new double[] { 0, 0, 0 };
				for (int i = 0; i < 3; i++) {
					if (t3[i] < 0)
						t3[i] += 1.0;
					if (t3[i] > 1)
						t3[i] -= 1.0;
					if (6.0 * t3[i] < 1.0)
						clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
					else if (2.0 * t3[i] < 1.0)
						clr[i] = temp2;
					else if (3.0 * t3[i] < 2.0)
						clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
					else
						clr[i] = temp1;
				}

				r = clr[0];
				g = clr[1];
				b = clr[2];
			}
			int red = (int)(255 * r);
			int green = (int)(255 * g);
			int blue = (int)(255 * b);

			red = (red > 255 ? 255 : (red < 0 ? 0 : red));
			green = (green > 255 ? 255 : (green < 0 ? 0 : green));
			blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));

			return Color.FromArgb(red, green, blue);
		}
	}
}
