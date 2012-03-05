using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Caliburn.Micro;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;
using YuvKA.ViewModel;

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
			IObservable<Frame> frames = new PipelineDriver().RenderTicks(new[] { graph }, tickCount: input.TickCount)
				.Select(dic => dic[graph.Outputs[0]]);
			YuvEncoder.Encode(
				@"..\..\..\..\output\ViewlessPipeline_sif.yuv",
				frames.ToEnumerable()
			);
		}

		[Fact]
		public void ViewfulPipeline()
		{
			var input = new VideoInputNode {
				FileName = new FilePath(@"..\..\..\..\resources\americanFootball_352x240_125.yuv")
			};
			Func<Node, Node> makeNode = n => {
				Node n2 = new InverterNode();
				n2.Inputs[0].Source = n.Outputs[0];
				return n2;
			};
			Node graph = makeNode(makeNode(makeNode(input)));

			IoC.GetInstance = delegate { return new EventAggregator(); };
			var output = new VideoOutputViewModel(graph.Outputs[0]);

			var sw = Stopwatch.StartNew();
			new PipelineDriver().RenderTicks(new[] { graph }, tickCount: input.TickCount)
				.ForEach(dic => output.Handle(new TickRenderedMessage(dic)));
			Console.WriteLine(sw.ElapsedMilliseconds + " ms");
		}
	}
}
