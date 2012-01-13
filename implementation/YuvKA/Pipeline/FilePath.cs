using System.Runtime.Serialization;
namespace YuvKA.Pipeline
{
	[DataContract]
	public sealed class FilePath
	{
		public FilePath(string path)
		{
			Path = path;
		}

		[DataMember]
		public string Path { get; private set; }
	}
}
