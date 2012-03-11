using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	/// <summary>
	/// This class prepares the Data of a
	/// DisplayNode to be displayed by the View
	/// </summary>
	public class DisplayViewModel : OutputWindowViewModel
	{
		int widthOld;
		int heightOld;

		public DisplayViewModel(DisplayNode node)
			: base(node, null)
		{
			NodeModel = node;
		}

		/// <summary>
		/// The Bitmap of the resultFrame, ready to be fetched by other classes.
		/// </summary>
		public WriteableBitmap RenderedImage { get; private set; }

		public new DisplayNode NodeModel { get; private set; }

		public override void Handle(TickRenderedMessage message)
		{
			base.Handle(message);
			if (NodeModel.Data == null)
				return;

			int width = NodeModel.Data.Size.Width;
			int height = NodeModel.Data.Size.Height;
			if (width == 0 || height == 0)
				return;

			if (RenderedImage == null || width != widthOld || height != heightOld) {
				RenderedImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
				widthOld = width;
				heightOld = height;
				NotifyOfPropertyChange(() => RenderedImage);
			}
			RenderedImage.Lock();

			unsafe {
				int* backBuffer = (int*)RenderedImage.BackBuffer;

				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						// Compute colors in pixel format. That is sRGB:
						// MSDN: Bgr32 is a sRGB format with 32 bits per pixel (BPP).
						// Each color channel (blue, green, and red) is allocated 8 bits per pixel (BPP).
						int rgb = ((NodeModel.Data[x, y].R << 16) |
								   (NodeModel.Data[x, y].G << 8) |
								   (NodeModel.Data[x, y].B));

						// Set the pixel at the current position to the BGR of the frame
						*backBuffer++ = rgb;
					}
				}
			}
			RenderedImage.AddDirtyRect(new Int32Rect(0, 0, width, height));
			RenderedImage.Unlock();
		}
	}
}