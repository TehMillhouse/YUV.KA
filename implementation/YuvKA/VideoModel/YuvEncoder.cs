using System;
using System.Collections.Generic;
using System.IO;

namespace YuvKA.VideoModel
{
	public static class YuvEncoder
	{
		public static Video Decode(string fileName, string logFileName, int width, int height)
		{
			// Nothing to see here, move along.
			return new Video(fileName, logFileName, new Size(width, height));
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
					// TODO These coefficients yield results different from those shown in
					// our canonical examples. That ought to be fixed sooner or later.
					// If I ever have the time, remind me to disassemble seqview
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

		/// <summary>
		/// Reads raw Yuv frames from the given file
		/// </summary>
		/// <param name="fileName">The file to read from</param>
		/// <param name="startFrame">The frame index to start reading </param>
		/// <param name="imageSize">The frame size to expect</param>
		/// <param name="frameCount">The number of frames to read</param>
		/// <returns>A byte array with the requested data</returns>
		private static byte[] ReadYuvFrames(string fileName, int startFrame, Size imageSize, int frameCount)
		{
			// Since a Yuv frame in our format has both U and V frames with only 1/4 the size
			// of the Y frame, the total frame size in bytes is Ysize + 2( 1/4 Ysize )
			int yuvFrameSize = (int)(imageSize.Height * imageSize.Width * 1.5);
			FileInfo fi = new FileInfo(fileName);
			if (!fi.Exists) {
				throw new FileNotFoundException();
			}
			if (fi.Length < (yuvFrameSize * (startFrame + frameCount))) {
				// Someone's trying to fetch a frame that isn't there
				throw new IndexOutOfRangeException();
			}
			byte[] data = new byte[yuvFrameSize * frameCount];
			using (FileStream stream = new FileStream(fileName, FileMode.Open)) {
				stream.Seek(yuvFrameSize * startFrame, SeekOrigin.Begin);
				stream.Read(data, 0, yuvFrameSize * frameCount);
			}
			return data;
		}

		#region Video class

		public class Video : IDisposable
		{
			// Maximal number of frames of original video to keep in memory
			// this is not necessarily equal to the actual size of the cache.
			// Therefor, always use frameCache.Length
			private const int MaxCacheSize = 10;
			private int? cachedBaseTick;
			private Frame[] frameCache;
			private Size frameSize;
			private string fileName;
			private string logFileName;

			/// <summary>
			/// Ctor for for the Video object. At this point, it is expected
			/// that the resolution of the video be known
			/// </summary>
			/// <param name="fileName">
			/// The name of the file to read. This has to be a valid
			/// yuv file, otherwise things go wrong. Throws FileNotFoundException.
			/// </param>
			/// <param name="logFileName">
			/// Optional. The log file to use for the yuv video
			/// </param>
			/// <param name="size">
			/// The resolution at which to read the frames from the file.
			/// Since the yuv format bears no metadata whatsoever, this has to be correct in order
			/// for the video to be outputted correctly.
			/// </param>
			public Video(string fileName, string logFileName, Size size)
			{
				frameSize = size;
				cachedBaseTick = null;
				this.fileName = fileName;
				FileInfo file = new FileInfo(fileName);
				if (!file.Exists) {
					throw new FileNotFoundException();
				}
				if (logFileName != null) {
					FileInfo log = new FileInfo(logFileName);
					if (log.Exists) {
						this.logFileName = logFileName;
					}
				}
				// Calculate number of frames in the yuv file
				FrameCount = (int)file.Length / (size.Width * size.Height + size.Width * size.Height / 2);
				// We don't want to use an array bigger than the video in the first place
				frameCache = new Frame[Math.Min(MaxCacheSize, FrameCount)];
				// Note that the array is not filled with meaningful data at this point.
				// This happens when the first frame is requested from the object.
			}

			public int FrameCount { get; private set; }

			// Indexer so we can access the different video frames
			// as if the Video were an array of frames
			public Frame this[int index]
			{
				get {
					// If the requested frame is not in the cache, load it from file
					int yuvFrameSize = (int)(frameSize.Height * frameSize.Width * 1.5);
					// If the cache is larger than the remaining number of frames in the video, we can't fetch them all
					int fetchFramesCount = (index >= FrameCount / frameCache.Length * frameCache.Length) ?
						(FrameCount % frameCache.Length)
						: (frameCache.Length);

					if ((cachedBaseTick == null) || (index >= cachedBaseTick + fetchFramesCount) || (index < cachedBaseTick)) {
						// requested frame is not cached, so we read enough data from file
						byte[] yuvData = ReadYuvFrames(fileName, index, frameSize, fetchFramesCount);
						byte[] frameData = new byte[yuvFrameSize];
						for (int i = 0; i < fetchFramesCount; i++) {
							// copy data for one frame into temporary array
							Array.Copy(yuvData, yuvFrameSize * i, frameData, 0, yuvFrameSize);
							// commit the frame we can calculate from this to cache
							frameCache[i] = Yuv2Rgb(frameData, frameSize.Width, frameSize.Height);
						}
						cachedBaseTick = index;
						return frameCache[0];
					}
					return frameCache[index - (int) cachedBaseTick];
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