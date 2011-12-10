﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class ImageInputNode : InputNode
	{
		[DisplayName("File Name")]
		public FilePath FileName { get; set; }

		public override Frame[] ProcessFrame(Frame[] inputs, int frameIndex)
		{
			throw new NotImplementedException();
		}
	}
}