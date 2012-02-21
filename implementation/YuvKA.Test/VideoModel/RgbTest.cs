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
		[Fact]
		public void TestEquality()
		{
			Rgb a = new Rgb(1, 2, 3);
			Rgb b = new Rgb(1, 2, 3);
			Rgb c = new Rgb(1, 2, 2);
			Assert.Equal(true, a == b);
			Assert.Equal(true, a.Equals(b));
			Assert.Equal(true, a != c);
			Assert.Equal(false, a == c);
			Assert.Equal(false, a.Equals(c));
			Assert.Equal(false, a != b);
		}
	}
}
