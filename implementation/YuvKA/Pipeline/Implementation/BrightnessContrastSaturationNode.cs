using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;

namespace YuvKA.Pipeline.Implementation
{
	[Description("Changes brightness, contrast, and/or saturation of the input")]
	public class BrightnessContrastSaturationNode : Node
	{
		[Range(-1.0, 1.0)]
		public double Brightness
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[Range(-1.0, 1.0)]
		public double Contrast
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[Range(-1.0, 1.0)]
		public double Saturation
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}
	}
}
