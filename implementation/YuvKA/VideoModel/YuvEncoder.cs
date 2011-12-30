using System;
using System.Collections.Generic;
using System.IO;

namespace YuvKA.VideoModel
{
	public static class YuvEncoder
	{
		public static Video Decode(string fileName, string logFileName, int width, int height)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Helper method for converting a bunch of data from YUV to a single RGB frame
		/// The method supports the IYUV / YUV420 format, and assumes that data array
		/// size, width and height make sense.
		/// </summary>
		public static Frame Yuv2Rgb(byte[] data, int width, int height)
		{
			// TODO make this private once testing is over
			int pixelNum = width * height;
			int quartSize = width * height / 4;
			Rgb[] frameData = new Rgb[height * width];
			int ypixel, upixel, vpixel;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					int coordOffset = y * width + x;
					// Our data format it IYUV / YUV420:
					// first all Y values, then all U value, and then all V values
					// the Y 'frame' is twice as big as the U and V 'frames', as
					// the human eye is better at recognizing luminance than it is at
					// distinguishing between different chromacities.

					// Get YUV data from given dataset
					ypixel = data[coordOffset];
					upixel = data[pixelNum + ((width / 2) * (y / 2) + x / 2)];
					vpixel = data[(pixelNum + quartSize) + ((width / 2) * (y / 2)  + x / 2)];

					// Convert data to RGB values
					// YCrCb conversion as described by YuvTools
					byte r = (byte)Math.Max(Math.Min(1.164 * (ypixel - 16) + 1.793 * (vpixel - 128), 255), 0);
					byte g = (byte)Math.Min(Math.Max(1.164 * (ypixel - 16) - 0.391 * (upixel - 128) - 0.813 * (vpixel - 128), 0), 255);
					byte b = (byte)Math.Max(Math.Min(1.164 * (ypixel - 16) + 2.018 * (upixel - 128), 255), 0);
					frameData[coordOffset] = new Rgb(r, g, b);
				}
			}
			return new Frame(new Size(width, height), frameData);
		}

		public static void Encode(string fileName, IEnumerable<Frame> frames)
		{
			throw new System.NotImplementedException();
		}

		#region Video class

		public class Video : IDisposable
		{
			// Number of frames of original video to keep in memory
			private const int MemFrames = 10;
			// private Frame[] frames;
			// private Size frameSize;
			// private string fileName;

			public Video(string fileName, string logFileName, int width, int height)
			{
				throw new NotImplementedException();
			}

			public int FrameCount { get; private set; }

			// Indexer so we can access the different video frames
			// as if the Video were an array of frames
			public Frame this[int index]
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			#endregion
		}
		#endregion
	}
}