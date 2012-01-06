using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;

namespace YuvKA.Test.Pipeline
{
	public class RgbSplitNodeTests
	{
		[Fact]
		public void TestAdditiveMerge()
		{
			Size testSize = new Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x * y), (byte)(x ^ y));
				}
			}
			RgbSplitNode rgbSplit = new RgbSplitNode();
			Frame[] result = rgbSplit.Process(inputs, 0);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal(result[0][x, y], new Rgb((byte)(x + y), 0, 0));
					Assert.Equal(result[1][x, y], new Rgb(0, (byte)(x * y), 0));
					Assert.Equal(result[2][x, y], new Rgb(0, 0, (byte)(x ^ y)));
				}
			}
		}
	}
}
