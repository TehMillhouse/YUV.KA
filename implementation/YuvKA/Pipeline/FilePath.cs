namespace YuvKA.Pipeline
{
	public sealed class FilePath
	{
		public FilePath(string path)
		{
			Path = path;
		}

		public string Path { get; private set; }
	}
}
