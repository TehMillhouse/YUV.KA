using System;
using System.Reactive.Linq;
using System.Threading;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;

namespace YuvKA.Test
{
	/// <summary>
	/// The results of these tests will have to be inspected manually. We're just making sure
	/// they don't throw any exceptions.
	/// </summary>
	public class IntegrationTests
	{
		[Fact]
		public void ViewlessPipeline()
		{
			var input = new VideoInputNode {
				FileName = new FilePath(@"..\..\..\..\resources\americanFootball_352x240_125.yuv")
			};
			Node graph = new BrightnessContrastSaturationNode { Contrast = 10 }; //new BlurNode { Radius = 3 };
			graph.Inputs[0].Source = input.Outputs[0];
			IObservable<Frame> frames = new PipelineDriver().RenderTicks(new[] { graph }, 0, new CancellationTokenSource()).Take(3).Select(dic => dic[graph.Outputs[0]]);
			YuvEncoder.Encode(
				@"..\..\..\..\output\ViewlessPipeline_sif.yuv",
				frames.ToEnumerable()
			);
		}
	}
}
