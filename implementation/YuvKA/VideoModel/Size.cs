using System.Runtime.Serialization;
namespace YuvKA.VideoModel
{
	[DataContract]
	public class Size
	{
		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}

		[DataMember]
		public int Width { get; private set; }

		[DataMember]
		public int Height { get; private set; }
	}
}
