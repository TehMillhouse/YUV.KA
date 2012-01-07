using System.Drawing;
using Xunit;
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
			inputNode.Size = new YuvKA.VideoModel.Size(100, 80);
			inputNode.FileName = new YuvKA.Pipeline.FilePath("..\\..\\..\\..\\resources\\bmp.png");
			Frame outputFrame = inputNode.OutputFrame(0);

			Bitmap outputImage = new Bitmap(inputNode.Size.Width, inputNode.Size.Height);
			for (int y = 0; y < outputFrame.Size.Height; ++y) {
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					outputImage.SetPixel(x, y, Color.FromArgb(outputFrame[x, y].R, outputFrame[x, y].G, outputFrame[x, y].B));
				}
			}
			outputImage.Save("..\\..\\..\\..\\output\\bmp-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Enlarge the image
			inputNode.Size = new YuvKA.VideoModel.Size(200, 400);
			outputFrame = inputNode.OutputFrame(0);
			outputImage = new Bitmap(inputNode.Size.Width, inputNode.Size.Height);
			for (int y = 0; y < outputFrame.Size.Height; ++y) {
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					outputImage.SetPixel(x, y, Color.FromArgb(outputFrame[x, y].R, outputFrame[x, y].G, outputFrame[x, y].B));
				}
			}
			outputImage.Save("..\\..\\..\\..\\output\\bmp-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			Assert.True(true);
		}
	}
}
