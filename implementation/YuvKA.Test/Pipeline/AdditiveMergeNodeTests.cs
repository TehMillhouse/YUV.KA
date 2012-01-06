namespace YuvKA.Test.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Xunit;
	using YuvKA.Pipeline.Implementation;
	using YuvKA.VideoModel;

	public class AdditiveMergeNodeTests
	{
		[Fact]
		public void TestAdditiveMerge()
		{
			Size testSize = new Size(5, 5);
			Frame[] inputs = { new Frame(testSize), new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
					inputs[1][x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
				}
			}
			AdditiveMergeNode addMerNode = new AdditiveMergeNode();
			Frame[] result = addMerNode.Process(inputs, 0);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal(result[0][x, y].R, inputs[0][x, y].R + inputs[1][x, y].R);
					Assert.Equal(result[0][x, y].G, inputs[0][x, y].G + inputs[1][x, y].G);
					Assert.Equal(result[0][x, y].B, inputs[0][x, y].B + inputs[1][x, y].B);
				}
			}
		}

	}
}
