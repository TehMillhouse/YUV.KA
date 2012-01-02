using System;
using System.Collections;
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
		/// Tests the Video object's ability to load videos from yuv files, convert them, and provide correct frame objects
		/// </summary>
		[Fact]
		public void TestVideoFunctionality()
		{
			int width = 352;
			int height = 240;
			string fileName = "..\\..\\..\\..\\resources\\americanFootball_352x240_125.yuv"; // be sure to adjust this beforehand
			string saveName = "..\\..\\..\\..\\output\\"; // warning, depending on the file, this produces a lot of images
			// leave the file extension away
			YuvEncoder.Video video = new YuvEncoder.Video(fileName, null, new VideoModel.Size(width, height));
			// Don't expect this to work if the filename doesn't happen to be formatted
			// just the right way
			Assert.Equal(video.FrameCount, int.Parse(fileName.Substring(fileName.Length - 7, 3)));

			Bitmap bmp;
			// I test the first and last three frames
			for (int i = 0; i < 3; i++) {
				bmp = Frame2Bitmap(video[i]);
				bmp.Save(saveName + i.ToString() + ".png");
			}
			for (int i = video.FrameCount - 4; i < video.FrameCount; i++) {
				bmp = Frame2Bitmap(video[i]);
				bmp.Save(saveName + i.ToString() + ".png");
			}
		}

		/// <summary>
		/// Tests the YuvEncoder's ability to decode and encode videos.
		/// If sucessfull, this test should procude a correct yuv file
		/// identical (or very close) to the one read from file
		/// </summary>
		[Fact]
		public void TestYuvEncoder()
		{
			int width = 352;
			int height = 240;
			string fileName = "..\\..\\..\\..\\resources\\americanFootball_352x240_125.yuv"; // be sure to adjust this beforehand
			string saveName = "..\\..\\..\\..\\output\\output.yuv"; // warning, depending on the file, this produces a lot of images
			YuvEncoder.Video video = new YuvEncoder.Video(fileName, null, new VideoModel.Size(width, height));
			IEnumerable<Frame> frameList = Enumerable.Range(0, video.FrameCount).Select(i => video[i]);
			YuvEncoder.Encode(saveName, frameList);
		}

		/// <summary>
		/// Does several passes of Decode/Encode over the same image.
		/// The point of this is to see how much distortion comes in and
		/// how much information is lost when en- or decoding.
		/// Reuires YuvEncoder.YuvToRgb and YuvEncoder.RgbToYuv to be made public
		/// </summary>
		[Fact]
		public void YuvEncoderStresstest()
		{
			// processes the same image many times
			int width = 352;
			int height = 240;
			VideoModel.Size size = new VideoModel.Size(width, height);

			string source = "..\\..\\..\\..\\resources\\americanFootball_352x240_125.yuv";
			string interim = "..\\..\\..\\..\\output\\interim.yuv";
			string finalFile = "..\\..\\..\\..\\output\\erroneous.png";

			string load = source;
			string save = interim;
			YuvEncoder.Video video = new YuvEncoder.Video(load, null, size);
			Frame herp = video[79];
			for (int i = 0; i < 20; i++) {
				// herp = YuvEncoder.YuvToRgb(YuvEncoder.RgbToYuv(herp), width, height);
			}
			Bitmap bmp = Frame2Bitmap(herp);
			bmp.Save(finalFile);
		}


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
