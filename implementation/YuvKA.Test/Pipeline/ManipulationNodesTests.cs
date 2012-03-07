using System.Collections.ObjectModel;

namespace YuvKA.Test.Pipeline
{
	using Xunit;
	using YuvKA.Pipeline;
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

		/// <summary>
		/// Ensures that the RGB Split node works correctly and that it doesn't mix up the color channels
		/// </summary>
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

		/// <summary>
		/// Asserts that the additive merge node works correctly
		/// </summary>
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

		/// <summary>
		/// Asserts that the weighted averaged merge node uses the correct weighting when combining input data
		/// </summary>
		[Fact]
		public void TestWeightedAveragedMerge()
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
			Node.Input testInput = new Node.Input();
			node.Inputs.Add(testInput);
			ObservableCollection<double> testGetWeights = node.Weights;
			// node.Weights is null -> create weights with default value 1.0
			Assert.Contains(1.0, testGetWeights);

			node.Inputs.Add(testInput);
			node.Weights[0] = 0;
			node.Weights[1] = 0.25;
			node.Inputs.Add(testInput);
			testGetWeights = node.Weights;
			// node.Weights has not enough values -> fill up missing weights with 1.0
			Assert.Contains(1.0, testGetWeights);

			Frame[] result = node.Process(inputs, 0);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal((byte)((0.25 * inputs[1][x, y].R + inputs[2][x, y].R) / 1.25), result[0][x, y].R);
					Assert.Equal((byte)((0.25 * inputs[1][x, y].G + inputs[2][x, y].G) / 1.25), result[0][x, y].G);
					Assert.Equal((byte)((0.25 * inputs[1][x, y].B + inputs[2][x, y].B) / 1.25), result[0][x, y].B);
				}
			}

			WeightedAveragedMergeNode secondNode = new WeightedAveragedMergeNode { Weights = { 0.75, 0.75, 0.75 } };
			node.Weights[0] = node.Weights[1] = node.Weights[2] = 0.5;
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

		/// <summary>
		/// Asserts that the inverter node works correctly
		/// </summary>
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

		/// <summary>
		/// Reads an image and applies the contrast effect of the BCS-Node to it, then saves it back to file
		/// </summary>
		[Fact]
		public void TestBrightnessContrastSaturation()
		{
			System.Drawing.Bitmap image = new System.Drawing.Bitmap("..\\..\\..\\..\\resources\\papagei.png");
			YuvKA.VideoModel.Size size = new YuvKA.VideoModel.Size(image.Width, image.Height);
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

			bcsNode.Contrast = 0.8;
			// Process the input frame. Reuse the frames object by writing back to it
			Frame[] outputFrames = bcsNode.Process(inputFrames, 0);

			// Copy RGB content of the processed frame to the output image
			for (int y = 0; y < size.Height; y++) {
				for (int x = 0; x < size.Width; ++x) {
					// Reuse the created image object
					image.SetPixel(x, y, System.Drawing.Color.FromArgb(outputFrames[0][x, y].R, outputFrames[0][x, y].G, outputFrames[0][x, y].B));
				}
			}

			image.Save("..\\..\\..\\..\\output\\papagei-bcs-" + (int)(bcsNode.Contrast * 10) + ".png");

			// Set contrast to a negative value and brightness to the maximum
			bcsNode.Contrast = -0.6;
			bcsNode.Brightness = 1;
			outputFrames = bcsNode.Process(inputFrames, 0);

			for (int y = 0; y < size.Height; y++) {
				for (int x = 0; x < size.Width; ++x) {
					// Reuse the created image object
					image.SetPixel(x, y, System.Drawing.Color.FromArgb(outputFrames[0][x, y].R, outputFrames[0][x, y].G, outputFrames[0][x, y].B));
				}
			}

			bcsNode.Brightness = -1;
			bcsNode.Process(inputFrames, 0);

			image.Save("..\\..\\..\\..\\output\\papagei-bcs-" + (int)(bcsNode.Contrast * 10) + ".png");
		}

		/// <summary>
		/// Creates Frames and test if they are delayed properly
		/// </summary>
		[Fact]
		public void TestDelayNode()
		{
			int n = 50;
			Frame[][] input = new Frame[n][];
			for (int i = 0; i < n; i++) {
				input[i] = new Frame[1];
				input[i][0] = new Frame(new Size(1, 1));
				input[i][0][0, 0] = new Rgb((byte)i, (byte)i, (byte)i);
			}
			for (int k = 0; k < 31; k++) {
				DelayNode node = new DelayNode();
				node.Delay = k;
				for (int i = 0; i < n; i++) {
					Frame[] output = node.Process(input[i], 0);
					if (i >= k)
						Assert.Equal(input[i - k][0], output[0]);
				}
			}
		}

		/// <summary>
		/// Creates two Frames and checks if their difference (and the
		/// difference of one frame with itself) is appropriate
		/// </summary>
		[Fact]
		public void TestDifferenceNode()
		{
			Size testSize = new Size(5, 5);
			Frame inputA = new Frame(testSize);
			Frame inputB = new Frame(testSize);
			Frame black = new Frame(testSize);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputA[x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
					inputB[x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
				}
			}
			DifferenceNode diffNode = new DifferenceNode();
			Frame[] diffReal = { inputA, inputB };
			Frame[] diffSelf = { inputA, inputA };
			Frame[] resultReal = diffNode.Process(diffReal, 0);
			Frame[] resultSelf = diffNode.Process(diffSelf, 0);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					Assert.Equal(127, resultSelf[0][x, y].R);
					Assert.Equal(127, resultSelf[0][x, y].G);
					Assert.Equal(127, resultSelf[0][x, y].B);
					Assert.Equal(127 + (((int)inputA[x, y].R - inputB[x, y].R) / 2), resultReal[0][x, y].R);
					Assert.Equal(127 + (((int)inputA[x, y].G - inputB[x, y].G) / 2), resultReal[0][x, y].G);
					Assert.Equal(127 + (((int)inputA[x, y].B - inputB[x, y].B) / 2), resultReal[0][x, y].B);
				}
			}
		}
	}
}
