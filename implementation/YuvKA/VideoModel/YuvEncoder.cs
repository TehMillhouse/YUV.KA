﻿using System;
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
					// distinguishing different chromacities.

					// Get YUV data from given dataset
					ypixel = data[coordOffset];
					upixel = data[pixelNum + (width * y / 4 + x / 2)];
					vpixel = data[(pixelNum + quartSize) + (width * y / 4 + x / 2)];

					// Convert data to RGB values
					byte r = (byte) Math.Min(ypixel + 1.14 * vpixel, 255);
					byte g = (byte) Math.Max(ypixel - 0.394 * upixel - 0.581 * vpixel, 0);
					byte b = (byte) Math.Min(ypixel + 2.032 * upixel, 255);
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
			// private Size frameSize;
			// private String fileName;
			public int FrameCount { get; set; }

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