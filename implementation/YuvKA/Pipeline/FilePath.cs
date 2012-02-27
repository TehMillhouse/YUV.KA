using System.Runtime.Serialization;
namespace YuvKA.Pipeline
{
	/// <summary>
	/// Class to type file paths more strictly than just System.String.
	/// This disambiguation is needed by the property type driven PropertyEditor.
	/// </summary>
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
