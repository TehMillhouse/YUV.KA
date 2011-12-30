using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using YuvKA.VideoModel;

namespace YuvKA.Test.Pipeline
{
	public class YuvEncoderTest
	{
		/// <summary>
		/// This just tests the Yuv2Rgb conversion for a given file.
		/// It converts the first frame of a yuv video and saves it as an image
		/// </summary>
		[Fact]
		public void TestYuvConversion()
		{
			int width = 352;
			int height = 288; // be sure to adjust this when running the test with your yuv file, lest there be colorful gibberish
			string loadFile = ""; // Insert path to your yuv
			string saveFile = ""; // Insert path to where you want the converted version saved
			FileStream fs = new FileStream(loadFile, FileMode.Open);
			byte[] data = new byte[fs.Length];
			fs.Read(data, 0, (int)fs.Length);
			fs.Close();
			Frame frame = YuvEncoder.Yuv2Rgb(data, width, height);
			Bitmap bmp = new Bitmap(width, height);
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					bmp.SetPixel(x, y, Color.FromArgb(frame[x, y].R, frame[x, y].G, frame[x, y].B));
			bmp.Save(saveFile);
		}
	}
}
