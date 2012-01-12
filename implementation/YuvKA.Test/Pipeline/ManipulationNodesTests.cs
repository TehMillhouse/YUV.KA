using System.Collections.ObjectModel;

namespace YuvKA.Test.Pipeline
{
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
			Frame[] testFrame = { new Frame(new YuvKA.VideoModel.Size(5, 5)) };
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
			Frame[] testFrame = { new Frame(new YuvKA.VideoModel.Size(5, 5)) };
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
			Frame[] testFrame = { new Frame(new YuvKA.VideoModel.Size(5, 5)) };
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
			Frame[] testFrame = { new Frame(new YuvKA.VideoModel.Size(5, 5)) };
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
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
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
					Assert.Equal(new Rgb((byte)(x + y), 0, 0), result[0][x, y]);
					Assert.Equal(new Rgb(0, (byte)(x * y), 0), result[1][x, y]);
					Assert.Equal(new Rgb(0, 0, (byte)(x ^ y)), result[2][x, y]);
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
					Assert.Equal(inputs[0][x, y].R + inputs[1][x, y].R, result[0][x, y].R);
					Assert.Equal(inputs[0][x, y].G + inputs[1][x, y].G, result[0][x, y].G);
					Assert.Equal(inputs[0][x, y].B + inputs[1][x, y].B, result[0][x, y].B);
				}
			}
		}


		[Fact]
		public void TestWeightedAverageMerge()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize), new Frame(testSize), new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
					inputs[1][x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
					inputs[2][x, y] = new Rgb((byte)(x ^ y), (byte)(x ^ y), (byte)(x ^ y));
				}
			}

			WeightedAveragedMergeNode node = new WeightedAveragedMergeNode();
			node.Weights = new ObservableCollection<double> { 0, 0.25, 1 };
			Frame[] result = node.Process(inputs, 0);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal((byte)((0.25 * inputs[1][x, y].R + inputs[2][x, y].R) / 1.25), result[0][x, y].R);
					Assert.Equal((byte)((0.25 * inputs[1][x, y].G + inputs[2][x, y].G) / 1.25), result[0][x, y].G);
					Assert.Equal((byte)((0.25 * inputs[1][x, y].B + inputs[2][x, y].B) / 1.25), result[0][x, y].B);
				}
			}

			WeightedAveragedMergeNode secondNode = new WeightedAveragedMergeNode();
			node.Weights = new ObservableCollection<double> { 0.5, 0.5, 0.5 };
			secondNode.Weights = new ObservableCollection<double> { 0.75, 0.75, 0.75 };
			result = node.Process(inputs, 0);
			Frame[] secondResult = secondNode.Process(inputs, 0);

			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal(result[0][x, y].R, secondResult[0][x, y].R);
					Assert.Equal(result[0][x, y].G, secondResult[0][x, y].G);
					Assert.Equal(result[0][x, y].B, secondResult[0][x, y].B);
				}
			}
		}

		[Fact]
		public void TestInverter()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			InverterNode inverter = new InverterNode();
			Frame[] result = inverter.Process(inputs, 0);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal(255 - inputs[0][x, y].R, result[0][x, y].R);
					Assert.Equal(255 - inputs[0][x, y].G, result[0][x, y].G);
					Assert.Equal(255 - inputs[0][x, y].B, result[0][x, y].B);
				}
			}
		}

		// Reads an image and applies the contrast effect of the BCS-Node to it, then saves it back to file
		[Fact]
		public void TestBrightnessContrastSaturation()
		{
			System.Drawing.Bitmap image = new System.Drawing.Bitmap("..\\..\\..\\..\\resources\\papagei.png");
			YuvKA.VideoModel.Size size = new VideoModel.Size(image.Width, image.Height);
			Frame[] inputFrames = { new Frame(size) };
			BrightnessContrastSaturationNode bcsNode = new BrightnessContrastSaturationNode();

			// Copy RGB content to the input frame
			for (int y = 0; y < size.Height; y++) {
				for (int x = 0; x < size.Width; ++x) {
					inputFrames[0][x, y] = new Rgb(image.GetPixel(x, y).R,
											  image.GetPixel(x, y).G,
											  image.GetPixel(x, y).B);
				}
			}

			bcsNode.Contrast = 2.0;
			// Process the input frame. Reuse the frames object by writing back to it
			Frame[] outputFrames = bcsNode.Process(inputFrames, 0);

			// Copy RGB content of the processed frame to the output image
			for (int y = 0; y < size.Height; y++) {
				for (int x = 0; x < size.Width; ++x) {
					// Reuse the created image object
					image.SetPixel(x, y, System.Drawing.Color.FromArgb(outputFrames[0][x, y].R,
														outputFrames[0][x, y].G,
														outputFrames[0][x, y].B));
				}
			}

			image.Save("..\\..\\..\\..\\output\\papagei-bcs-" + (int)(bcsNode.Contrast * 10) + ".png");
		}
	}
}
