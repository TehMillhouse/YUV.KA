using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class ImageInputNode : InputNode
	{
		Bitmap inputImage;
		Bitmap resizedInputImage;
		FilePath fileName;
		Frame resizedFrame;

		public ImageInputNode()
			: base(outputCount: 1)
		{
		}

		[DisplayName("File Name")]
		[DataMember]
		public FilePath FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				// If the path was changed, the image will have to be loaded again
				inputImage = null;
				// and the resized image recreated
				resizedInputImage = null;
			}
		}

		/// <summary>
		/// Returns a new Frame with the RGB data of the selected PNG image
		/// </summary>
		/// <param name="tick">Parameter ignored.</param>
		/// <returns>A Frame with the RGB information of the selected image.</returns>
		public override Frame OutputFrame(int tick)
		{
			EnsureInputLoaded();
			return resizedFrame;
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			// If the size was changed, the resized image will have to be created
			// from the original with the new dimensions.
			resizedInputImage = null;
		}

		private void EnsureInputLoaded()
		{
			// If the path was changed, we have to load the new image
			if (inputImage == null) {
				inputImage = new Bitmap(FileName.Path);
			}

			// If the size was changed
			if (resizedInputImage == null) {
				// If the size of the output frame differs from that of the input image, 
				// make a resized copy of the original
				if (inputImage.Width != Size.Width || inputImage.Height != Size.Height) {
					resizedInputImage = new Bitmap(inputImage, new System.Drawing.Size(Size.Width, Size.Height));
				}
				else {
					// Otherwise keep a reference to the original
					resizedInputImage = inputImage;
				}
			}

			// Create output frame
			resizedFrame = new Frame(new VideoModel.Size(Size.Width, Size.Height));
			for (int y = 0; y < Size.Height; ++y) {
				for (int x = 0; x < Size.Width; ++x) {
					resizedFrame[x, y] = new Rgb(resizedInputImage.GetPixel(x, y).R,
												 resizedInputImage.GetPixel(x, y).G,
												 resizedInputImage.GetPixel(x, y).B);
				}
			}
		}
	}
}
