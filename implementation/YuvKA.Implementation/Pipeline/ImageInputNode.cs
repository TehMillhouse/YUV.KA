using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class ImageInputNode : InputNode
	{
		[DisplayName("File Name")]
		[DataMember]
		public FilePath FileName { get; set; }

		public override Frame OutputFrame(int tick)
		{
			throw new NotImplementedException();
		}
	}
}
