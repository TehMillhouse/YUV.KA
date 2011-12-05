using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;

namespace YuvKA.Pipeline.Implementation
{
	public class ImageInputNode : InputNode
	{
		[DisplayName("File Name")]
		public FilePath FileName { get; set; }

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}
	}
}
