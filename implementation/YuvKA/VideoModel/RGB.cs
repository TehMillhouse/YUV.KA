namespace YuvKA.VideoModel
{
	public struct Rgb
	{
		public Rgb(byte r, byte g, byte b) : this()
		{
			R = r;
			G = g;
			B = b;
		}

		public byte R { get; private set; }
		public byte G { get; private set; }
		public byte B { get; private set; }

		public static bool operator ==(Rgb a, Rgb b)
		{
			// If both are null, or both are same instance, return true.
			if (object.ReferenceEquals(a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match:
			return a.R == b.R && a.G == b.G && a.B == b.B;
		}

		public static bool operator !=(Rgb a, Rgb b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return R ^ G ^ B;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Rgb))
				return false;
			Rgb pixel = (Rgb)obj;
			return (R == pixel.R && G == pixel.G && B == pixel.B);
		}
	}
}