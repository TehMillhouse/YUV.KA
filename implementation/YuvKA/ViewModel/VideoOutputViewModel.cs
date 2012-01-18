using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuvKA.Pipeline;

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
			}

			sourceImage.Lock();

			unsafe {
				int pBackBuffer = (int)sourceImage.BackBuffer;

				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						// Compute colors in pixel format. That is BGR.
						int bgr = ((message[Output][x, y].B << 16) |
								   (message[Output][x, y].G << 8) |
								   (message[Output][x, y].R));

						// Set the pixel at the current position to the BGR of the frame
						*((int*)pBackBuffer) = bgr;
						++pBackBuffer;
					}
				}
			}

			sourceImage.Unlock();
			NotifyOfPropertyChange(() => sourceImage);
		}

		//public void ShowSample()
		//{
		//    sourceImage = new BitmapImage(new System.Uri("D:\\YUV.KA\\resources\\sample.png"));
		//    NotifyOfPropertyChange(() => sourceImage);
		//}
	}
}
