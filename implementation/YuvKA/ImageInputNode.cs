using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace YuvKA
{
	public class ImageInputNode : InputNode
	{
		public string FileName { get; set; }

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}
	}
}
