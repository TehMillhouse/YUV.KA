using System;

namespace YuvKA.VideoModel
{
	public class Frame
	{
		private Rgb[] data;

		public Frame(Size size)
		{
			data = new Rgb[size.Height * size.Width];
			Size = size;
		}

		// Alternate constructor for saving the trouble of using the indexer for everything
		public Frame(Size size, Rgb[] data)
		{
			if (data.Length != size.Height * size.Width) {
				throw new System.ArgumentException();
			}
			this.data = data;
			Size = size;
		}

		public Size Size { get; private set; }

		public Rgb this[int x, int y]
		{
			get { return data[y * Size.Width + x]; }
			set { data[y * Size.Width + x] = value; }
		}

		public Rgb GetPixelOrBlack(int x, int y)
		{
			return (x < Size.Width && y < Size.Height) ? this[x, y] : new Rgb(0, 0, 0);
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
	}
}
