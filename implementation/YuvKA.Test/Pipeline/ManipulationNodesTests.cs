namespace YuvKA.Test.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Xunit;
	using YuvKA.Pipeline.Implementation;
	using YuvKA.VideoModel;

	public class ManipulationNodesTests
	{
		/// <summary>
		/// Blurring a monocolored Frame should not have any effect
		/// </summary>
		[Fact]
		public void TestLinearBlurMonocolor()
		{
			Frame[] testFrame = { new Frame(new Size(5, 5)) };
			for (int i = 0; i < 256; i++) {
				for (int x = 0; x < testFrame[0].Size.Width; x++) {
					for (int y = 0; y < testFrame[0].Size.Height; y++) {
						testFrame[0][x, y] = new Rgb((byte)i, (byte)i, (byte)i);
					}
				}
				BlurNode blur = new BlurNode();
				Frame[] result = blur.Process(testFrame, 0);
				for (int x = 0; x < testFrame[0].Size.Width; x++) {
					for (int y = 0; y < testFrame[0].Size.Height; y++) {
						Assert.Equal(testFrame[0][x, y], result[0][x, y]);
					}
				}
			}
		}

		/// <summary>
		/// Blurring a monocolored Frame should not have any effect
		/// </summary>
		[Fact]
		public void TestGaussianBlurMonocolor()
		{
			Frame[] testFrame = { new Frame(new Size(5, 5)) };
			for (int i = 0; i < 256; i++) {
				for (int x = 0; x < testFrame[0].Size.Width; x++) {
					for (int y = 0; y < testFrame[0].Size.Height; y++) {
						testFrame[0][x, y] = new Rgb((byte)i, (byte)i, (byte)i);
					}
				}
				BlurNode blur = new BlurNode();
				blur.Type = BlurType.Gaussian;
				Frame[] result = blur.Process(testFrame, 0);
				for (int x = 0; x < testFrame[0].Size.Width; x++) {
					for (int y = 0; y < testFrame[0].Size.Height; y++) {
						Assert.Equal(testFrame[0][x, y], result[0][x, y]);
					}
				}
			}
		}

		/// <summary>
		/// Blurring with Radius 0 should not have any effect
		/// </summary>
		[Fact]
		public void TestLinearZeroBlur()
		{
			Frame[] testFrame = { new Frame(new Size(5, 5)) };
			for (int x = 0; x < testFrame[0].Size.Width; x++) {
				for (int y = 0; y < testFrame[0].Size.Height; y++) {
					testFrame[0][x, y] = new Rgb((byte)(10 * x * y), (byte)(10 * x * y), (byte)(10 * x * y));
				}
			}
			BlurNode blur = new BlurNode();
			blur.Radius = 0;
			Frame[] result = blur.Process(testFrame, 0);
			for (int x = 0; x < testFrame[0].Size.Width; x++) {
				for (int y = 0; y < testFrame[0].Size.Height; y++) {
					Assert.Equal(testFrame[0][x, y], result[0][x, y]);
				}
			}
		}

		/// <summary>
		/// Blurring with Radius 0 should not have any effect
		/// </summary>
		[Fact]
		public void TestGaussianZeroBlur()
		{
			Frame[] testFrame = { new Frame(new Size(5, 5)) };
			for (int x = 0; x < testFrame[0].Size.Width; x++) {
				for (int y = 0; y < testFrame[0].Size.Height; y++) {
					testFrame[0][x, y] = new Rgb((byte)(10 * x * y), (byte)(10 * x * y), (byte)(10 * x * y));
				}
			}
			BlurNode blur = new BlurNode();
			blur.Type = BlurType.Gaussian;
			blur.Radius = 0;
			Frame[] result = blur.Process(testFrame, 0);
			for (int x = 0; x < testFrame[0].Size.Width; x++) {
				for (int y = 0; y < testFrame[0].Size.Height; y++) {
					Assert.Equal(testFrame[0][x, y], result[0][x, y]);
				}
			}
		}

		[Fact]
		public void TestRgbSplit()
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
