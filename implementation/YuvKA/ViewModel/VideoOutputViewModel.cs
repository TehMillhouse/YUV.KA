using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class VideoOutputViewModel : OutputWindowViewModel
	{
		public VideoOutputViewModel(Node.Output output)
			: base(output.Node, output)
		{
		}

		public WriteableBitmap SourceImage { get; set; }
		int widthOld;
		int heightOld;

		public override void Handle(TickRenderedMessage message)
		{
			base.Handle(message);
			var frame = message[OutputModel];
			if (frame == null)
				return;

			int width = frame.Size.Width;
			int height = frame.Size.Height;
			if (width == 0 || height == 0)
				return;

			if (SourceImage == null || width != widthOld || height != heightOld) {
				SourceImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
				widthOld = width;
				heightOld = height;
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
						int bgr = ((frame[x, y].R << 16) |
								  (frame[x, y].G << 8) |
								   (frame[x, y].B));

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
