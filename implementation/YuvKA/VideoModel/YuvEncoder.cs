using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace YuvKA.VideoModel
{
	/// <summary>
	/// The YuvEncoder provides a simple static interface for converting between Frame objects in RGB color space and yuv files on disk.
	/// See Decode and Encode methods for further details.
	/// </summary>
	public static class YuvEncoder
	{
		/// <summary>
		/// Reads a yuv-file from disk and converts is into a Video object of RGB color formatting.
		/// If present, log and movement vector metadata is used to enhance the returned video.
		/// Since the yuv420 format doesn't carry any metadata, the height and width parameters are mandatory.
		/// </summary>
		/// <param name="width">The frame width to assume</param>
		/// <param name="height">The frame height to assume</param>
		/// <param name="fileName">The path to the yuv file to be read</param>
		/// <param name="logFileName">
		/// The path to any log information previously produced by an encoder.
		/// If present, this log information will be incorporated into the Video object returned.
		/// </param>
		/// <param name="motionVectorFileName">
		/// The path to any motion vector information previously produced by an encoder.
		/// If present, these motion vectors will be incorporated into the Video object returned.
		/// </param>
		/// <returns>
		/// A Video object that can be queried for frames.
		/// </returns>
		public static Video Decode(int width, int height, string fileName, string logFileName = null, string motionVectorFileName = null)
		{
			// Nothing to see here, move along.
			return new Video(new Size(width, height), fileName, logFileName, motionVectorFileName);
		}

		/// <summary>
		/// Takes an IEnumerable of RGB frames and encodes it into a yuv file under the given file path.
		/// </summary>
		public static void Encode(string fileName, IEnumerable<Frame> frames)
		{
			using (var stream = new FileStream(fileName, FileMode.Create))
				Encode(stream, frames);
		}

		/// <summary>
		/// Takes an IEnumerable of RGB frames and encodes it into a given data stream.
		/// </summary>
		public static void Encode(Stream stream, IEnumerable<Frame> frames)
		{
			foreach (Frame frame in frames) {
				byte[] yuvData = RgbToYuv(frame);
				stream.Write(yuvData, 0, yuvData.Length);
			}
		}

		#region Helper methods

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
			FileInfo file = new FileInfo(fileName);
			if (!file.Exists) {
				throw new FileNotFoundException();
			}
			if (file.Length < (yuvFrameSize * (startFrame + frameCount))) {
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

		/// <summary>
		/// Helper method for converting a bunch of data from YUV to a single RGB frame
		/// The method supports the IYUV / YUV420 format, and assumes that data array
		/// size, width and height make sense.
		/// </summary>
		/// <param name="data">
		/// The raw Yuv data that constitutes the frame
		/// </param>
		private static Frame YuvToRgb(byte[] data, int width, int height)
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
					// distinguishing between different chromacities.

					// Get YUV data from given dataset
					ypixel = data[coordOffset];
					upixel = data[pixelNum + ((width / 2) * (y / 2) + x / 2)];
					vpixel = data[(pixelNum + quartSize) + ((width / 2) * (y / 2) + x / 2)];

					// Convert data to RGB values
					// YCrCb conversion as described by ITU-R 601, with tweaked coefficients
					byte r = ClampToByte(1.167 * (ypixel - 16) + 1.596 * (vpixel - 128));
					byte g = ClampToByte(1.169 * (ypixel - 16) - 0.393 * (upixel - 128) - 0.816 * (vpixel - 128));
					byte b = ClampToByte(1.167 * (ypixel - 16) + 2.018 * (upixel - 128));
					frameData[coordOffset] = new Rgb(r, g, b);
				}
			}
			return new Frame(new Size(width, height), frameData);
		}

		/// <summary>
		/// Converts an arbitraty value of double precision to a byte by cropping information
		/// </summary>
		private static byte ClampToByte(double value)
		{
			return (byte)Math.Max(Math.Min(value, 255), 0);
		}

		/// <summary>
		/// Helper method for converting a Frame object into raw yuv data that can be saved to file
		/// Operates under the assumption that the frame width and height are divisible by 2
		/// This can be made a lot more efficient, currently uses (w*h * 1.5) steps, can be done in (w*h)
		/// </summary>
		private static byte[] RgbToYuv(Frame inputFrame)
		{
			int yuvDataSize = (int)(inputFrame.Size.Height * inputFrame.Size.Width * 1.5);
			byte[] yuvData = new byte[yuvDataSize];

			int y, x;
			// fill Y data frame
			for (y = 0; y < inputFrame.Size.Height; y++) {
				for (x = 0; x < inputFrame.Size.Width; x++) {
					// This formula is taken from the wikipedia article for YCbCr
					// It's the ITU-R 601 version, but hand-tweaked.
					// This is optimized for readability, not speed
					int r = inputFrame[x, y].R;
					int g = inputFrame[x, y].G;
					int b = inputFrame[x, y].B;
					yuvData[y * inputFrame.Size.Width + x] = ClampToByte(16 + (65.738 * r / 256) + (129.657 * g / 256) + (25.064 * b / 256));
				}
			}

			// fill U and V data frames
			int offset = inputFrame.Size.Width * inputFrame.Size.Height;
			int smallOfset = offset / 4;
			for (y = 0; y < inputFrame.Size.Height / 2; y++) {
				for (x = 0; x < inputFrame.Size.Width / 2; x++) {
					// since the U and V data frames are only a quarter the size of the RGB version, we need to average the
					// 4 pixels that will be saved as one in order not to lose too much information
					int r = (inputFrame[2 * x, 2 * y].R + inputFrame[2 * x + 1, 2 * y].R + inputFrame[2 * x, 2 * y + 1].R + inputFrame[2 * x + 1, 2 * y + 1].R) / 4;
					int g = (inputFrame[2 * x, 2 * y].G + inputFrame[2 * x + 1, 2 * y].G + inputFrame[2 * x, 2 * y + 1].G + inputFrame[2 * x + 1, 2 * y + 1].G) / 4;
					int b = (inputFrame[2 * x, 2 * y].B + inputFrame[2 * x + 1, 2 * y].B + inputFrame[2 * x, 2 * y + 1].B + inputFrame[2 * x + 1, 2 * y + 1].B) / 4;
					byte value = ClampToByte(128 + (-37.945 * r / 256) - (74.394 * g / 256) + (112.439 * b / 256));
					yuvData[offset + y * inputFrame.Size.Width / 2 + x] = value;
					value = ClampToByte(128 + (112.439 * r / 256) - (94.074 * g / 256) - (18.285 * b / 256));
					yuvData[offset + smallOfset + y * inputFrame.Size.Width / 2 + x] = value;
				}
			}
			return yuvData;
		}

		/// <summary>
		/// Reads the log data from file and returns it as a byte array.
		/// The array returned by this function may not have the right size or content if the given file doesn't contain valid log information
		/// </summary>
		/// <param name="logFileName">
		/// The File to read from
		/// </param>
		/// <returns>
		/// A raw byte array containing the data read from file
		/// </returns>
		private static byte[] ReadLogData(string logFileName)
		{
			FileInfo logFile = new FileInfo(logFileName);
			if (!logFile.Exists) {
				throw new FileNotFoundException();
			}
			//Reads the whole File into that byte array
			byte[] data = new byte[logFile.Length];
			using (FileStream stream = new FileStream(logFileName, FileMode.Open)) {
				stream.Read(data, 0, (int)logFile.Length);
			}
			return data;
		}

		/// <summary>
		/// Reads movement vector information from file found at given location.
		/// The file that's to be read should be supplied in csv (comma separated value) format.
		/// Data will not be valid or complete if the file at the given location isn't valid or complete.
		/// </summary>
		/// <param name="fileName">
		/// The path to the file that's to be parsed
		/// </param>
		/// <returns>
		/// An array of arrays containing the movement vector data.
		/// The first level hereof indexes the lines/frames of data read from file,
		/// the second level indexes the single data points read from the csv file.
		/// </returns>
		private static int[][] ReadMovementVectors(string fileName)
		{
			FileInfo vectorFile = new FileInfo(fileName);
			if (!vectorFile.Exists) {
				throw new FileNotFoundException();
			}
			/* Read the csv file in a two level int array
			 * First level is the Frame/line of the data
			 * Second level is data for the vectors of each macroblock*/
			string[] inputLines = File.ReadAllLines(fileName);
			int[][] data = new int[inputLines.Length][];
			for (int i = 0; i < inputLines.Length; i++) {
				string[] line = inputLines[i].Split(',');
				data[i] = new int[line.Length];
				for (int j = 0; j < line.Length - 1; j++) {
					if (line[j] == "")
						data[i][j] = 0;
					else
						data[i][j] = int.Parse(line[j]);
				}
			}
			return data;
		}

		/// <summary>
		/// Adds log information and movement vector metadata to a given frame
		/// </summary>
		/// <param name="frame">
		/// The basic frame to be enhanced with metadata
		/// </param>
		/// <param name="macroblockPartitionData">
		/// A byte array containing the macroblock decision information to be added to the frame.
		/// Invalid values yield undefined behavior.
		/// </param>
		/// <param name="vectorData">
		/// An array of arrays containing the vector data to be added to the frame.
		/// If not enough data is present, the two-dimensional zero vector (0,0) is used for all remaining macroblocks.
		/// </param>
		/// <param name="index">
		/// The index of the given frame in the video stream.
		/// Used for selecting the right metadata for the given frame.
		/// </param>
		/// <returns>
		/// An instance of AnnotatedFrame bearing the given parameters as source of information.
		/// </returns>
		private static AnnotatedFrame AddAnnotations(Frame frame, byte[] macroblockPartitionData, int[][] vectorData, int index)
		{
			int macroBlockNumber = frame.Size.Width / 16 * frame.Size.Height / 16;
			MacroblockDecision[] decisions = new MacroblockDecision[macroBlockNumber];
			for (int i = 0; i < decisions.Length; i++) {
				decisions[i] = new MacroblockDecision();
			}
			if (macroblockPartitionData != null) {
				for (int i = 0; i < decisions.Length && macroBlockNumber * index + i < macroblockPartitionData.Length; i++) {
					decisions[i].PartitioningDecision = (MacroblockPartitioning)macroblockPartitionData[macroBlockNumber * index + i];
				}
			}
			if (vectorData != null) {
				for (int i = 0; i < decisions.Length; i++) {
					// if we run out of vectors just pretend there are plenty zero vectors
					if (index < vectorData.Length && vectorData[index].Length > i * 2 + 1) {
						decisions[i].Movement = new Vector(vectorData[index][i * 2], vectorData[index][i * 2 + 1]);
					}
					else {
						decisions[i].Movement = new Vector(0, 0);
					}
				}
			}
			return new AnnotatedFrame(frame, decisions);
		}

		#endregion Helper methods

		#region Video class

		/// <summary>
		/// The Video class represents a loaded video.
		/// It loads frames one by one from disk without keeping the file stream open as to let background file operations work.
		/// It interfaces with the YuvEncoder for converting between the yuv420 and RGB formats,
		/// and handles adding metadata to Frame objects.
		/// </summary>
		public class Video
		{
			private int? lastTick;
			private Frame frameCache;
			private Size frameSize;
			private string fileName;
			private string logFileName = null;
			private byte[] logFile = null;
			private string motionVectorFileName = null;
			private int[][] motionVectorFile = null;

			/// <summary>
			/// Ctor for for the Video object. At this point, it is expected
			/// that the resolution of the video be known
			/// </summary>
			/// <param name="size">
			/// The resolution at which to read the frames from the file.
			/// Since the yuv format bears no metadata whatsoever, this has to be correct in order
			/// for the video to be outputted correctly.
			/// </param>
			/// <param name="fileName">
			/// The name of the file to read. This has to be a valid
			/// yuv file, otherwise things go wrong. Throws FileNotFoundException.
			/// </param>
			/// <param name="logFileName">
			/// Optional. The log file to use for the yuv video
			/// </param>
			/// <param name="motionVectorFileName">
			/// Optional. The motion vector file to use for the yuv video
			/// </param>
			public Video(Size size, string fileName, string logFileName, string motionVectorFileName)
			{
				frameSize = size;
				lastTick = null;
				this.fileName = fileName;
				FileInfo file = new FileInfo(fileName);
				if (!file.Exists) {
					throw new FileNotFoundException();
				}
				// Calculate number of frames in the yuv file
				FrameCount = (int)file.Length / (size.Width * size.Height + size.Width * size.Height / 2);
				// if we have a logfile parse and cache it
				if (logFileName != null && File.Exists(logFileName)) {
					this.logFileName = logFileName;
					this.logFile = ReadLogData(logFileName);
				}
				// if we have a vectorfile parse and cache it
				if (motionVectorFileName != null && File.Exists(motionVectorFileName)) {
					this.motionVectorFileName = motionVectorFileName;
					this.motionVectorFile = ReadMovementVectors(motionVectorFileName);
				}
			}

			public int FrameCount { get; private set; }

			/// <summary>
			/// Indexer for video frames so the video can be accessed like an array.
			/// Throws IndexOutOfRangeException when an invalid index is chosen.
			/// </summary>
			/// <returns>
			/// The Frame at the given index.
			/// </returns>
			public Frame this[int index]
			{
				get
				{
					if (index < 0 || index >= FrameCount) {
						throw new IndexOutOfRangeException();
					}
					if (index != lastTick) {
						frameCache = YuvToRgb(ReadYuvFrames(fileName, index, frameSize, 1), frameSize.Width, frameSize.Height);
						if (logFileName != null || motionVectorFileName != null) {
							frameCache = AddAnnotations(frameCache, logFile, motionVectorFile, index);
						}
						lastTick = index;
					}
					return frameCache;
				}
			}
		}
		#endregion
	}
}