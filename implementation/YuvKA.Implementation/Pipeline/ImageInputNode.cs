using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Provides a video stream contaning a still image from a PNG file.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Rarely instantiated, long-living class")]
	[DataContract]
	[Description("This Node provides a video stream contaning a still image from a PNG file")]
	public class ImageInputNode : InputNode
	{
		Bitmap inputImage;
		Bitmap resizedInputImage;
		FilePath fileName = new FilePath(null);
		Frame resizedFrame;

		/// <summary>
		/// Creates a new ImageInputNode
		/// </summary>
		public ImageInputNode()
			: base(outputCount: 1)
		{
			Name = "Image";
			fileName = new FilePath(null);
		}

		/// <summary>
		/// Gets or sets the path of the PNG image file to be streamed into the pipeline
		/// </summary>
		[DisplayName("File Name")]
		[FilePath.ExtensionFilter("PNG File|*.png|All files (*.*)|*")]
		[DataMember]
		[Browsable(true)]
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

		public override bool InputIsValid
		{
			get
			{
				return fileName.Path != null;
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
			if (inputImage != null && resizedInputImage != null) {
				// The data we've got is still valid
				return;
			}
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
			resizedFrame = new Frame(new YuvKA.VideoModel.Size(Size.Width, Size.Height));
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
