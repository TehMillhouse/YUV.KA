using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public static class HslHelper
	{
		/// <summary>
		/// Convertes a color from the HSL to the RGB color space.
		/// </summary>
		/// <param name="h">The hue of the HSL color</param>
		/// <param name="s">The saturation of the HSL color</param>
		/// <param name="l">The lightness of the HSL color</param>
		/// <returns>A RGB Color object transformed from the given color in the HSL space</returns>
		public static Color HslToRgb(double h, double s, double l)
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
