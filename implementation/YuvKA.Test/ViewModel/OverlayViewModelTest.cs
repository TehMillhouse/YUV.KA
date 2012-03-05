using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;
using YuvKA.ViewModel;
namespace YuvKA.Test.ViewModel
{
	public class OverlayViewModelTest
	{
		/// <summary>
		/// This test is a general all-round test for the OverlayViewModel
		/// </summary>
		[Fact]
		public void TestGeneralFunctions()
		{
			IOverlayType noOverlay = new NoOverlay();
			IOverlayType blockOverlay = new BlocksOverlay();
			IoC.GetAllInstances = nope => new List<object> { noOverlay, blockOverlay };
			IoC.GetInstance = IoCAggregator;
			OverlayNode node = new OverlayNode();
			OverlayViewModel vm = new OverlayViewModel(node);

			var inputNodeMock = new Mock<InputNode>(1);
			inputNodeMock.SetupGet(i => i.OutputHasLogfile).Returns(true);
			inputNodeMock.SetupGet(i => i.InputIsValid).Returns(true);

			//Test the available Types
			node.Inputs[0].Source = inputNodeMock.Object.Outputs[0];
			Assert.Equal("No Overlay", vm.TypeTuples.First().Item1);

			//Test altering the chosen Type
			node.Type = blockOverlay;
			Assert.Equal("Macroblock-Overlay", vm.ChosenType.Item1);
			ColorInputNode color = new ColorInputNode();
			color.Size = new Size(512, 512);
			node.Inputs[0].Source = color.Outputs[0];
			vm.ChosenType = new System.Tuple<string, IOverlayType>("No Overlay", noOverlay);
			Assert.Equal("No Overlay", vm.ChosenType.Item1);
			//Test "Handling" a message(The result has be inspected manually)
			node.Type = noOverlay;
			Frame[] input = { new Frame(new Size(512, 512)) };
			node.ProcessCore(input, 0);
			vm.Handle(null);

			using (var fileStream = new FileStream(@"..\..\..\..\output\thisIsBlack.png", FileMode.Create)) {
				BitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(vm.RenderedImage));
				encoder.Save(fileStream);
			}
		}

		private IEnumerable<object> IoCOverlayInstances(System.Type nope)
		{
			return new List<object> { new NoOverlay(), new BlocksOverlay() };
		}

		private object IoCAggregator(System.Type type, string str)
		{
			if (type == typeof(MainViewModel)) {
				return MainViewModelTest.GetInstance();
			}
			return new EventAggregator();
		}
	}
}
