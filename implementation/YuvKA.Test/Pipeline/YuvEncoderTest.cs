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
			int height = 240; // be sure to adjust this when running the test with your yuv file, lest there be colorful gibberish
			string loadFile = ""; // Insert path to your yuv
			string saveFile = ""; // Insert path to where you want the converted version saved
			FileStream fs = new FileStream(loadFile, FileMode.Open);
			byte[] data = new byte[fs.Length];
			fs.Read(data, 0, (int)fs.Length);
			fs.Close();
			Frame frame = YuvEncoder.Yuv2Rgb(data, width, height);
			Bitmap bmp = Frame2Bitmap(frame);
			bmp.Save(saveFile);
		}

		[Fact]
		public void TestVideoFunctionality()
		{
			int width = 352;
			int height = 240;
			string fileName = "...\\americanFootball_352x240_125.yuv"; // be sure to adjust this beforehand
			string saveName = ""; // warning, depending on the file, this produces a lot of images
			// leave the file extension away
			YuvEncoder.Video video = new YuvEncoder.Video(fileName, null, new VideoModel.Size(width, height));
			// Don't expect this to work if the filename doesn't happen to be formatted
			// just the right way
			Assert.Equal(video.FrameCount, int.Parse(fileName.Substring(fileName.Length - 7, 3)));

			Bitmap bmp;
			for (int i = 0; i < video.FrameCount; i++) {
				bmp = Frame2Bitmap(video[i]);
				bmp.Save(saveName + i.ToString() + ".png");
			}
		}

		[Fact(Skip = "This is just a helper method for testing")]
		private static Bitmap Frame2Bitmap(Frame frame)
		{
			Bitmap bmp = new Bitmap(frame.Size.Width, frame.Size.Height);
			for (int y = 0; y < frame.Size.Height; y++)
				for (int x = 0; x < frame.Size.Width; x++)
					bmp.SetPixel(x, y, Color.FromArgb(frame[x, y].R, frame[x, y].G, frame[x, y].B));
			return bmp;
		}
	}
}
