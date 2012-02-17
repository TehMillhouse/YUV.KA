using System;

namespace YuvKA.VideoModel
{
	/// <summary>
	/// The Frame class represents a single video frame in the RGB color space.
	/// It has preset width and height and can be accessed like a two-dimensional array.
	/// </summary>
	public class Frame
	{
		private Rgb[] data;

		/// <summary>
		/// Constructor that only takes a Size as argument. Data is allocated but not assigned yet, and thus undefined.
		/// </summary>
		/// <param name="size">
		/// A Size object specifying the frame size
		/// </param>
		public Frame(Size size)
		{
			data = new Rgb[size.Height * size.Width];
			Size = size;
		}

		/// <summary>
		/// Copy constructor for Frame object.
		/// Take heed: this is only a shallow copy. If you modify the copy's data, you modify the original.
		/// </summary>
		/// <param name="frame">
		/// The frame reference to be copied
		/// </param>
		public Frame(Frame frame)
		{
			data = frame.data;
			Size = frame.Size;
		}

		/// <summary>
		/// Alternate constructor for saving the trouble of using the indexer for setting up a frame with already well formed data
		/// </summary>
		/// <param name="size">The supposed frame size</param>
		/// <param name="data">The frame data</param>
		public Frame(Size size, Rgb[] data)
		{
			if (data.Length != size.Height * size.Width) {
				throw new System.ArgumentException();
			}
			this.data = data;
			Size = size;
		}

		public Size Size { get; private set; }

		/// <summary>
		/// Indexer for enabling arraylike access to pixels.
		/// </summary>
		/// <returns>The Rgb object at given coordinates, if those coordinates are valid</returns>
		public Rgb this[int x, int y]
		{
			get { return data[y * Size.Width + x]; }
			set { data[y * Size.Width + x] = value; }
		}

		/// <summary>
		/// Returns the largest boundaries found in the specified frame array, so all frame sizes are smaller than the returned one.
		/// </summary>
		/// <param name="frames">The specified frame array.</param>
		/// <returns>The size object containing the larges boundaries.</returns>
		public static Size MaxBoundaries(Frame[] frames)
		{
			int maxX = 0;
			int maxY = 0;
			foreach (Frame frame in frames) {
				maxX = Math.Max(maxX, frame.Size.Width);
				maxY = Math.Max(maxY, frame.Size.Height);
			}
			return new Size(maxX, maxY);
		}

		/// <summary>
		/// An accessor that ensures the returned value is always a valid Rgb object.
		/// </summary>
		/// <returns>
		/// The Rgb object specifying the frame data at the given coordinates if those are valid.
		/// If the given coordinates are outside of the image frame, a black pixel value is returned instead.
		/// </returns>
		public Rgb GetPixelOrBlack(int x, int y)
		{
			return (x < Size.Width && y < Size.Height && x >= 0 && y >= 0) ? this[x, y] : new Rgb(0, 0, 0);
		}
	}
}
