using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class VideoOutputViewModel : OutputWindowViewModel
	{
		public VideoOutputViewModel(Node.Output output)
			: base(output.Node)
		{
			Output = output;
		}

		public Node.Output Output { get; private set; }
		public WriteableBitmap SourceImage { get; set; }

		public override void Handle(TickRenderedMessage message)
		{
			base.Handle(message);
			if (message[Output] == null)
				return;

			int width = message[Output].Size.Width;
			int height = message[Output].Size.Height;
			if (width == 0 || height == 0)
				return;

			if (SourceImage == null) {
				SourceImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
				NotifyOfPropertyChange(() => SourceImage);
			}

			SourceImage.Lock();

			unsafe {
				int* backBuffer = (int*)SourceImage.BackBuffer;

				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						// Compute colors in pixel format. That is sRGB:
						// MSDN: Bgr32 is a sRGB format with 32 bits per pixel (BPP).
						// Each color channel (blue, green, and red) is allocated 8 bits per pixel (BPP).
						int bgr = ((message[Output][x, y].R << 16) |
								   (message[Output][x, y].G << 8) |
								   (message[Output][x, y].B));

						// Set the pixel at the current position to the BGR of the frame
						*backBuffer++ = bgr;
					}
				}
			}

			SourceImage.AddDirtyRect(new Int32Rect(0, 0, width, height));
			SourceImage.Unlock();
		}
	}
}
