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

		[Fact]
		public void TestYuvEndoder()
		{
			int width = 352;
			int height = 240;
			string fileName = "..\\..\\..\\..\\resources\\americanFootball_352x240_125.yuv"; // be sure to adjust this beforehand
			string saveName = "..\\..\\..\\..\\output\\output.yuv"; // warning, depending on the file, this produces a lot of images
			YuvEncoder.Video video = new YuvEncoder.Video(fileName, null, new VideoModel.Size(width, height));
			EnumerableVideo frameList = new EnumerableVideo(video);
			YuvEncoder.Encode(saveName, frameList);
		}

		private static Bitmap Frame2Bitmap(Frame frame)
		{
			Bitmap bmp = new Bitmap(frame.Size.Width, frame.Size.Height);
			for (int y = 0; y < frame.Size.Height; y++)
				for (int x = 0; x < frame.Size.Width; x++)
					bmp.SetPixel(x, y, Color.FromArgb(frame[x, y].R, frame[x, y].G, frame[x, y].B));
			return bmp;
		}

		/// <summary>
		/// Implementation of IEnumerable needed to test the YuvEncoder on its own.
		/// There's probably something only Sebastian knows that lets me convert this
		/// custom data structure we have into an IEnumerable, but I can't find it, so there.
		/// I would advise against using it beyond basic testing, as I didn't really know
		/// what I was doing here. Please kill me now.
		/// </summary>
		private class EnumerableVideo : IEnumerable<Frame>
		{
			private YuvEncoder.Video video;

			public EnumerableVideo(YuvEncoder.Video video)
			{
				this.video = video;
			}

			IEnumerator<Frame> IEnumerable<Frame>.GetEnumerator()
			{
				return (IEnumerator<Frame>)GetEnumerator();
			}

			public VideoEnumerator GetEnumerator()
			{
				return new VideoEnumerator(video);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			public class VideoEnumerator : IEnumerator<Frame>
			{
				int position = -1;
				private YuvEncoder.Video video;
				public VideoEnumerator(YuvEncoder.Video video)
				{
					this.video = video;
				}

				public Frame Current
				{
					get
					{
						return video[position];
					}
				}

				object IEnumerator.Current { get { return Current; } }

				public bool MoveNext()
				{
					position++;
					return (position < video.FrameCount);
				}

				public void Reset()
				{
					position = -1;
				}

				public void Dispose() { }
			}
		}
	}
}
