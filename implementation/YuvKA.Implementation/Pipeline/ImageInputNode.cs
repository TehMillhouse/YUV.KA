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
		FilePath fileName;

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
				inputImage = null;
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
			Frame outputFrame = new Frame(new YuvKA.VideoModel.Size(inputImage.Width, inputImage.Height));

			for (int y = 0; y < outputFrame.Size.Height; ++y) {
				for (int x = 0; x < outputFrame.Size.Width; ++x) {
					outputFrame[x, y] = new Rgb(inputImage.GetPixel(x, y).R, inputImage.GetPixel(x, y).G, inputImage.GetPixel(x, y).B);
				}
			}

			return outputFrame;
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			if (inputImage != null) {
				inputImage = new Bitmap(inputImage, new System.Drawing.Size(Size.Width, Size.Height));
			}
		}

		private void EnsureInputLoaded()
		{
			if (inputImage == null) {
				inputImage = new Bitmap(FileName.Path);

				if (inputImage.Height != Size.Height || inputImage.Width != Size.Width) {
					OnSizeChanged();
				}
			}
		}
	}
}
