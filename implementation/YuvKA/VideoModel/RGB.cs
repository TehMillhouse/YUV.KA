namespace YuvKA.VideoModel
{
	/// <summary>
	/// A class representing the color data inside a pixel in red, green and blue values.
	/// </summary>
	public struct Rgb
	{
		/// <summary>
		/// A simple constructor
		/// </summary>
		/// <param name="r">The value for the red component</param>
		/// <param name="g">The value for the green component</param>
		/// <param name="b">The value for the blue component</param>
		public Rgb(byte r, byte g, byte b)
			: this()
		{
			R = r;
			G = g;
			B = b;
		}

		public byte R { get; private set; }
		public byte G { get; private set; }
		public byte B { get; private set; }

		/// <summary>
		/// Overrides the == of Rgb, to compare the Rgb values.
		/// </summary>
		/// <param name="a">An Rgb object</param>
		/// <param name="b">Another Rgb object </param>
		/// <returns>True if the two Rgb Objects have the same values in R,G and B.
		/// False otherwise.</returns>
		public static bool operator ==(Rgb a, Rgb b)
		{
			// Return true if the fields match:
			return a.R == b.R && a.G == b.G && a.B == b.B;
		}

		/// <summary>
		/// Overrides the != of Rgb, to compare the Rgb values.
		/// </summary>
		/// <param name="a">An Rgb object</param>
		/// <param name="b">Another Rgb object </param>
		/// <returns>True if the two Rgb Objects have different values in R,G or B.
		/// False otherwise.</returns>
		public static bool operator !=(Rgb a, Rgb b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current Object.</returns>
		public override int GetHashCode()
		{
			return R ^ G ^ B;
		}

		/// <summary>
		/// Overrides the equals-method of Rgb, to compare the Rgb values.
		/// </summary>
		/// <param name="obj">An object</param>
		/// <returns>True if the given Rgb Object is Rgb and has the same values in R,G and B as this object.
		/// False otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Rgb))
				return false;
			Rgb pixel = (Rgb)obj;
			return (R == pixel.R && G == pixel.G && B == pixel.B);
		}

		/// <summary>
		/// Returns a human-readable representation of the pixel color value
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("(R {0} G {1} B {2})", R, G, B);
		}
	}
}