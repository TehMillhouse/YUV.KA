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
			if (data.Length != size.Height * size.Width)
			{
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
	}
}
