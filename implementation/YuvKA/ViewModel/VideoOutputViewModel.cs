using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuvKA.Pipeline;
using System.Windows;

namespace YuvKA.ViewModel
{
	public class VideoOutputViewModel : OutputWindowViewModel
	{
		public Node.Output Output { get; private set; }
		public WriteableBitmap sourceImage { get; set; }

		public VideoOutputViewModel(Node.Output output) : base(output.Node)
		{
			Output = output;
		}

		public override void Handle(TickRenderedMessage message)
		{
			base.Handle(message);
			int width = message[Output].Size.Width;
			int height = message[Output].Size.Height;
			if (sourceImage == null) {
				sourceImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
				NotifyOfPropertyChange(() => sourceImage);
			}

			sourceImage.Lock();

			unsafe {
				int* pBackBuffer = (int*)sourceImage.BackBuffer;

				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						// Compute colors in pixel format. That is sRGB:
						// MSDN: Bgr32 is a sRGB format with 32 bits per pixel (BPP).
						// Each color channel (blue, green, and red) is allocated 8 bits per pixel (BPP).
						int bgr = ((message[Output][x, y].R << 16) |
								   (message[Output][x, y].G << 8) |
								   (message[Output][x, y].B));

						// Set the pixel at the current position to the BGR of the frame
						*pBackBuffer++ = bgr;
					}
				}
			}

			sourceImage.AddDirtyRect(new Int32Rect(0, 0, width, height));
			sourceImage.Unlock();
		}

		//public void ShowSample()
		//{
		//    sourceImage = new BitmapImage(new System.Uri("D:\\YUV.KA\\resources\\sample.png"));
		//    NotifyOfPropertyChange(() => sourceImage);
		//}
	}
}
