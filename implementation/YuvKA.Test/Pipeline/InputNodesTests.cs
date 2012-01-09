using System;
using System.Drawing;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;

namespace YuvKA.Test.Pipeline
{
	public class InputNodesTests
	{
		// Read a PNG file and resize it using the methods in ImageInputNode.
		// This test does no verification. It's intended to generate a result to be visualized.
		[Fact]
		public void ImageInputTest()
		{
			ImageInputNode inputNode = new ImageInputNode();
			Frame outputFrame;
			Bitmap outputImage;

			inputNode.FileName = new YuvKA.Pipeline.FilePath("..\\..\\..\\..\\resources\\bmp.png");

			// Reduce the image
			inputNode.Size = new YuvKA.VideoModel.Size(100, 80);
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\bmp-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Enlarge the image
			inputNode.Size = new YuvKA.VideoModel.Size(200, 400);
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\bmp-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Change path and size
			inputNode.Size = new VideoModel.Size(400, 50);
			inputNode.FileName = new YuvKA.Pipeline.FilePath("..\\..\\..\\..\\resources\\sample.png");
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\sample-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Change only size
			inputNode.Size = new VideoModel.Size(50, 400);
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\sample-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");
		}

		// Output a color and write it to a file
		// Test the order of resize and color change operations
		[Fact]
		public void ColorInputTest()
		{
			Frame outputFrame;
			Bitmap outputImage;
			ColorInputNode colorInput = new ColorInputNode();

			colorInput.Size = new VideoModel.Size(200, 200);
			colorInput.Color = new Rgb(50, 92, 177);
			CopyFrameToOutputImage(colorInput, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\color-50-92-177-200x200.png");

			// Change the size
			colorInput.Size = new VideoModel.Size(100, 50);
			CopyFrameToOutputImage(colorInput, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\color-50-92-177-100x50.png");

			// Change the color
			Color c = Color.Crimson;
			byte r = c.R, g = c.G, b = c.B;
			colorInput.Color = new Rgb(r, g, b);
			CopyFrameToOutputImage(colorInput, out outputFrame, out outputImage, 0);
			outputImage.Save("..\\..\\..\\..\\output\\color-" + r + "-" + g + "-" + b + "177-100x50.png");
		}

		// Create images with the two supported noise types: Perlin and Coherent noise
		[Fact]
		public static void NoiseInputTest()
		{
			Frame outputFrame;
			Bitmap outputImage;
			int tick;

			// Perlin noise
			NoiseInputNode noiseInput = new NoiseInputNode();
			noiseInput.Type = NoiseType.Perlin;
			noiseInput.Size = new VideoModel.Size(352, 240);
			tick = 7;

			DateTime start = DateTime.Now; // Start time of Perlin noise test
			outputFrame = noiseInput.OutputFrame(tick);
			DateTime end = DateTime.Now; // End time

			TimeSpan diff = end.Subtract(start);

			// Time limit for the Perlin noise should be 200 ms
			Assert.True(diff.Seconds * 1000 + diff.Milliseconds < 200);

			outputImage = new Bitmap(noiseInput.Size.Width, noiseInput.Size.Height);
			for (int y = 0; y < outputFrame.Size.Height; ++y) {
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					outputImage.SetPixel(x, y, Color.FromArgb(outputFrame[x, y].R,
															  outputFrame[x, y].G,
															  outputFrame[x, y].B));
				}
			}
			outputImage.Save("..\\..\\..\\..\\output\\noise-" + noiseInput.Type + "-tick-" + tick + "-352x240.png");

			// Change size and noise type to Coherent
			noiseInput.Size = new VideoModel.Size(200, 200);
			noiseInput.Type = NoiseType.Coherent;
			tick = 7;
			CopyFrameToOutputImage(noiseInput, out outputFrame, out outputImage, tick);
			outputImage.Save("..\\..\\..\\..\\output\\noise-" + noiseInput.Type + "-tick-" + tick + "-200x200.png");
		}

		// A helper method to save the frame to a PNG image file
		private static void CopyFrameToOutputImage(InputNode inputNode, out Frame outputFrame, out Bitmap outputImage, int tick)
		{
			outputFrame = inputNode.OutputFrame(tick);
			outputImage = new Bitmap(inputNode.Size.Width, inputNode.Size.Height);
			for (int y = 0; y < outputFrame.Size.Height; ++y) {
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					outputImage.SetPixel(x, y, Color.FromArgb(outputFrame[x, y].R,
															  outputFrame[x, y].G,
															  outputFrame[x, y].B));
				}
			}
		}
	}
}
