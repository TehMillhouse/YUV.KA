using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	[Description("Changes brightness, contrast, and/or saturation of the input")]
	public class BrightnessContrastSaturationNode : Node
	{
		[DataMember]
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

		[DataMember]
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

		[DataMember]
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

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
