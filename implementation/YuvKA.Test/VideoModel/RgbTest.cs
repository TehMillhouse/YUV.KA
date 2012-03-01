namespace YuvKA.Test.VideoModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Xunit;
	using YuvKA.Pipeline.Implementation;
	using YuvKA.VideoModel;

	public class RgbTest
	{
		/// <summary>
		/// Asserts that the overloaded operators work as intended
		/// </summary>
		[Fact]
		public void TestEquality()
		{
			Rgb a = new Rgb(1, 2, 3);
			Rgb b = new Rgb(1, 2, 3);
			Rgb c = new Rgb(1, 2, 2);
			Rgb d = new Rgb(1, 1, 3);
			Rgb e = new Rgb(2, 2, 3);
			Assert.Equal(true, a == b);
			Assert.Equal(true, a.Equals(b));
			Assert.Equal(true, a != c);
			Assert.Equal(false, a == c);
			Assert.Equal(false, a.Equals(c));
			Assert.Equal(false, a != b);
			Assert.Equal(false, a.Equals(null));
			Assert.Equal(false, a == d);
			Assert.Equal(false, a == e);
			Assert.Equal(false, a.Equals(d));
			Assert.Equal(false, a.Equals(e));
		}

		/// <summary>
		/// Insures that the hashing works correctly in the Rgb class
		/// </summary>
		[Fact]
		public void TestHashing()
		{
			Rgb a = new Rgb(1, 2, 3);
			Rgb b = new Rgb(1, 2, 3);
			Rgb c = new Rgb(1, 2, 2);
			Assert.Equal(a.GetHashCode(), b.GetHashCode());
			Assert.NotEqual(a.GetHashCode(), c.GetHashCode());
		}

		/// <summary>
		/// Asserts that the String representation works as intended
		/// </summary>
		[Fact]
		public void TestToString()
		{
			Rgb a = new Rgb(1, 2, 3);
			string s = "(R 1 G 2 B 3)";
			Assert.Equal(s, a.ToString());
		}
	}
}
