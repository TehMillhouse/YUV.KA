using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YuvKA.Pipeline;
using YuvKA.VideoModel;

namespace RGBMerge
{
	[Description("This node can merge different video streams into one, respecting color channels")]
	public class RgbMergeNode : Node
	{
		public RgbMergeNode()
			: base(inputCount: 3, outputCount: 1)
		{
			Name = "Rgb Merge";
		}

		public override YuvKA.VideoModel.Frame[] Process(YuvKA.VideoModel.Frame[] inputs, int tick)
		{
			Size maxSize = Frame.MaxBoundaries(inputs);
			Frame outputFrame = new Frame(maxSize);
			byte resultR;
			byte resultG;
			byte resultB;
			for (int x = 0; x < maxSize.Width; x++) {
				for (int y = 0; y < maxSize.Height; y++) {
					resultR = 0;
					resultG = 0;
					resultB = 0;

					resultR += inputs[0].GetPixelOrBlack(x, y).R;
					resultR += inputs[0].GetPixelOrBlack(x, y).G;
					resultR += inputs[0].GetPixelOrBlack(x, y).B;

					resultG += inputs[1].GetPixelOrBlack(x, y).R;
					resultG += inputs[1].GetPixelOrBlack(x, y).G;
					resultG += inputs[1].GetPixelOrBlack(x, y).B;

					resultB += inputs[2].GetPixelOrBlack(x, y).R;
					resultB += inputs[2].GetPixelOrBlack(x, y).G;
					resultB += inputs[2].GetPixelOrBlack(x, y).B;

					outputFrame[x, y] = new Rgb(resultR, resultG, resultB);
				}
			}
			return new[] { outputFrame };
		}
	}
}
