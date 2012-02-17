using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Xunit;
using YuvKA.VideoModel;
using System.Diagnostics;

namespace YuvKA.Test.ViewModel
{
	public class YuvEncoderTest
	{
		public static Bitmap FrameToBitmap(Frame frame)
		{
			Bitmap bmp = new Bitmap(frame.Size.Width, frame.Size.Height);
			for (int y = 0; y < frame.Size.Height; y++)
				for (int x = 0; x < frame.Size.Width; x++)
					bmp.SetPixel(x, y, Color.FromArgb(frame[x, y].R, frame[x, y].G, frame[x, y].B));
			return bmp;
		}

		/// <summary>
		/// Tests the Video object's ability to load videos from yuv files, convert them, and provide correct frame objects
		/// </summary>
		[Fact]
		public void TestVideoFunctionality()
		{
			int width = 352;
			int height = 240;
			string videoName = @"..\..\..\..\resources\americanFootball_352x240_125.yuv"; // be sure to adjust this beforehand
			string logFileName = @"..\..\..\..\resources\ModeGrid_AmFootball_OUR.dat";
			string motionVectorFileName = @"..\..\..\..\resources\motion_info\motion_info_football_qp20.csv";
			string saveName = @"..\..\..\..\output\";
			// leave the file extension away
			YuvEncoder.Video video = YuvEncoder.Decode(width, height, videoName, logFileName, motionVectorFileName);
			// Don't expect this to work if the filename doesn't happen to be formatted
			// just the right way
			Assert.Equal(video.FrameCount, int.Parse(videoName.Substring(videoName.Length - 7, 3)));
			// Test video and frame indexer
			Assert.Equal(new Rgb(0, 0, 0), video[0].GetPixelOrBlack(-1, -1));
			// obviously, this is only true if the video is either very large and black or not that large
			Assert.Equal(new Rgb(0, 0, 0), video[0].GetPixelOrBlack(9999999, 9999999));
			Assert.Equal(video[0][0, 0], video[0].GetPixelOrBlack(0, 0));

			Bitmap bmp;
			// I test the first and last three frames
			for (int i = 0; i < 3; i++) {
				bmp = FrameToBitmap(video[i]);
				bmp.Save(saveName + i.ToString() + ".png");
			}
			for (int i = video.FrameCount - 4; i < video.FrameCount; i++) {
				bmp = FrameToBitmap(video[i]);
				bmp.Save(saveName + i.ToString() + ".png");
			}
		}

		/// <summary>
		/// Tests the YuvEncoder's ability to decode and encode videos.
		/// If sucessfull, this test should produce a correct yuv file
		/// identical (or very close) to the one read from file
		/// </summary>
		[Fact]
		public void TestYuvEncoder()
		{
			int width = 352;
			int height = 240;
			string fileName = "..\\..\\..\\..\\resources\\americanFootball_352x240_125.yuv"; // be sure to adjust this beforehand
			string saveName = "..\\..\\..\\..\\output\\yuvencoder-output_352x240_125.yuv"; // warning, depending on the file, this produces a lot of images
			YuvEncoder.Video video = new YuvEncoder.Video(new VideoModel.Size(width, height), fileName, null, null);
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
			string finalFile = "..\\..\\..\\..\\output\\multipass.png";

			YuvEncoder.Video video = new YuvEncoder.Video(size, source, null, null);
			Frame frame = video[79];
			for (int i = 0; i < 20; i++) {
				// herp = YuvEncoder.YuvToRgb(YuvEncoder.RgbToYuv(herp), width, height);
			}
			Bitmap bmp = FrameToBitmap(frame);
			bmp.Save(finalFile);
		}

		/// <summary>
		/// Print out the time for Encoding one whole video,
		/// to have comparevalues when modifying the encoder
		/// </summary>
		[Fact]
		public void YuvEncoderSpeedtest()
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			int width = 352;
			int height = 240;
			string fileName = "..\\..\\..\\..\\resources\\americanFootball_352x240_125.yuv";
			YuvEncoder.Video video = new YuvEncoder.Video(new VideoModel.Size(width, height), fileName, null, null);
			Frame notLazy;
			for (int i = 0; i < 125; i++) {
				notLazy = video[i];
			}
			System.Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
		}
	}
}
