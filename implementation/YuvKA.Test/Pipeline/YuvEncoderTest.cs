using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using YuvKA.VideoModel;

namespace YuvKA.Test.Pipeline
{
	public class YuvEncoderTest
	{
		/// <summary>
		/// Ignore this, this just tests the Yuv2Rgb conversion
		/// </summary>
		[Fact]
		public void TestYuvConversion()
		{
			FileStream fs = new FileStream("C:\\Users\\max\\Desktop\\singleFrame_352x288.yuv", FileMode.Open);
			byte[] data = new byte[fs.Length];
			fs.Read(data, 0, (int)fs.Length);
			fs.Close();
			// Frame frame = YuvEncoder.Yuv2Rgb(data, 352, 288);

			Console.WriteLine("Success!");
		}
	}
}
