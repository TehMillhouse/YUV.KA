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
			Frame outputFrame;
			Bitmap outputImage;

			inputNode.FileName = new YuvKA.Pipeline.FilePath("..\\..\\..\\..\\resources\\bmp.png");

			// Reduce the image
			inputNode.Size = new YuvKA.VideoModel.Size(100, 80);
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage);
			outputImage.Save("..\\..\\..\\..\\output\\bmp-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Enlarge the image
			inputNode.Size = new YuvKA.VideoModel.Size(200, 400);
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage);
			outputImage.Save("..\\..\\..\\..\\output\\bmp-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Change path and size
			inputNode.Size = new VideoModel.Size(400, 50);
			inputNode.FileName = new YuvKA.Pipeline.FilePath("..\\..\\..\\..\\resources\\sample.png");
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage);
			outputImage.Save("..\\..\\..\\..\\output\\sample-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");

			// Change only size
			inputNode.Size = new VideoModel.Size(50, 400);
			CopyFrameToOutputImage(inputNode, out outputFrame, out outputImage);
			outputImage.Save("..\\..\\..\\..\\output\\sample-resized-" +
							inputNode.Size.Width + "-" + inputNode.Size.Height + ".png");
		}

		private static void CopyFrameToOutputImage(ImageInputNode inputNode, out Frame outputFrame, out Bitmap outputImage)
		{
			outputFrame = inputNode.OutputFrame(0);
			outputImage = new Bitmap(inputNode.Size.Width, inputNode.Size.Height);
			for (int y = 0; y < outputFrame.Size.Height; ++y) {
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					outputImage.SetPixel(x, y, Color.FromArgb(outputFrame[x, y].R, outputFrame[x, y].G, outputFrame[x, y].B));
				}
			}
		}
	}
}
