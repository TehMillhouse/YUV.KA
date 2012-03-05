using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Represents the View Model to the corresponding VideoOutputView.
	/// 
	/// It generates the frames that will be drawn to the user interface.
	/// </summary>
	public class VideoOutputViewModel : OutputWindowViewModel
	{
		/// <summary>
		/// Creates a new VideoOutputViewModel.
		/// </summary>
		public VideoOutputViewModel(Node.Output output)
			: base(output.Node, output)
		{
		}

		/// <summary>
		/// The image to be drawn to the user interface.
		/// </summary>
		public WriteableBitmap SourceImage { get; private set; }

		/// <summary>
		/// Writes the image to be drawn to the back buffer.
		/// </summary>
		/// <param name="message">The TickRenderedMessage received, which contains the frame to be drawn</param>
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

			if (SourceImage == null || width != SourceImage.Width || height != SourceImage.Height) {
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
