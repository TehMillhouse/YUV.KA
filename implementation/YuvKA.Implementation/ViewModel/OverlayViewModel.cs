using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	/// <summary>
	/// This class represents prepares the Data of an
	/// OverlayNode to be displayed by the View
	/// </summary>
	public class OverlayViewModel : OutputWindowViewModel
	{
		int widthOld;
		int heightOld;

		public OverlayViewModel(OverlayNode node)
			: base(node, null)
		{
			NodeModel = node;
		}

		/// <summary>
		/// The Bitmap of the resultFrame, ready to be fetched by other classes.
		/// </summary>
		public WriteableBitmap RenderedImage { get; private set; }

		/// <summary>
		/// The OverlayNode associated with this ViewModel.
		/// </summary>
		public new OverlayNode NodeModel { get; private set; }

		/// <summary>
		/// The collection of available Overlaynames and types.
		/// </summary>
		public IEnumerable<System.Tuple<string, IOverlayType>> TypeTuples
		{
			get
			{
				/* Get all available IOverlayTypes */
				ICollection<System.Tuple<string, IOverlayType>> overlayTypes = new List<System.Tuple<string, IOverlayType>>();
				foreach (IOverlayType type in IoC.GetAllInstances(typeof(IOverlayType))) {
					/* Validate Input for the Type */
					if (NodeModel.Inputs[0].Source != null
						&& (!type.DependsOnReference || NodeModel.Inputs[1].Source != null)
						&& (!type.DependsOnLogfiles || NodeModel.Inputs[0].Source.Node.OutputHasLogfile)
						&& (!type.DependsOnVectors || NodeModel.Inputs[0].Source.Node.OutputHasMotionVectors)) {
						overlayTypes.Add(new System.Tuple<string, IOverlayType>(type.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName, type));
					}
				}
				return overlayTypes;
			}
		}

		/// <summary>
		/// The chosen Overlayname and type.
		/// </summary>
		public System.Tuple<string, IOverlayType> ChosenType
		{
			get
			{
				return this.TypeTuples.First(i => NodeModel.Type.GetType() == i.Item2.GetType());
			}
			set
			{
				NodeModel.Type = value.Item2;
				MainViewModel mvm = (MainViewModel)IoC.Get<MainViewModel>();
				if (!mvm.Model.IsPlaying) {
					mvm.Model.RenderTick(new[] { this.NodeModel });
				}
			}
		}

		public override void Handle(TickRenderedMessage message)
		{
			base.Handle(message);
			int width = NodeModel.Data.Size.Width;
			int height = NodeModel.Data.Size.Height;
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
