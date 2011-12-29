namespace YuvKA.VideoModel
{
	public struct Rgb
	{
		byte r, g, b;

		public Rgb(byte r, byte g, byte b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public byte R
		{
			get { return r; }
			set { r = value; }
		}

		public byte G
		{
			get { return g; }
			set { g = value; }
		}

		public byte B
		{
			get { return b; }
			set { b = value; }
		}
	}
}
