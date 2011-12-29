namespace YuvKA.VideoModel
{
	public class Frame
	{
		Rgb[] data;

		public Frame(Size size)
		{
			data = new Rgb[size.Height * size.Width];
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
