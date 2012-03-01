using System.Threading;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.Pipeline;
using YuvKA.ViewModel;

namespace YuvKA.Test.ViewModel
{
	public class ReplayStateViewModelTest
	{
		ReplayStateViewModel vm = MainViewModelTest.GetInstance().ReplayStateViewModel;

		/// <summary>
		/// Asserts that the program detects invalid pipelines and thus disallows the user from playing it
		/// </summary>
		[Fact]
		public void CantPlayInvalidPipeline()
		{
			Assert.False(vm.IsPlaying);

			Node graph = new BlurNode();
			Assert.False(graph.InputIsValid);
			vm.Parent.OpenWindows.Clear();
			vm.Parent.OpenWindows.Add(new VideoOutputViewModel(graph.Outputs[0]));
			vm.PlayPause();
			Assert.False(vm.IsPlaying);
		}

		/// <summary>
		/// Asserts that pausing/resuming execution of the pipeline works
		/// </summary>
		[Fact]
		public void PlayPauseWorks()
		{
			vm.Stop();
			Assert.False(vm.IsPlaying);
			Assert.Equal(vm.Parent.Model.CurrentTick, 0);

			Node graph = new AnonymousNode(() => Thread.Sleep(1000));
			vm.Parent.OpenWindows.Clear();
			vm.Parent.OpenWindows.Add(new VideoOutputViewModel(graph.Outputs[0]));
			vm.PlayPause();
			Assert.True(vm.IsPlaying);
			Thread.Sleep(1500);
			vm.PlayPause();
			Assert.False(vm.IsPlaying);
			Assert.Equal(vm.Parent.Model.CurrentTick, 1);
		}

		/// <summary>
		/// Asserts that stopping and then playing the pipeline corretly starts the pipeline at the beginning of the video.
		/// </summary>
		[Fact]
		public void StopsAtEndThenStartsAnew()
		{
			vm.Stop();

			var graphMock = new Mock<InputNode>(/*outputCount*/ (int?)1) { CallBase = true };
			graphMock.SetupGet(n => n.TickCount).Returns(10);
			vm.Parent.Model.Graph.AddNode(graphMock.Object);

			var outputMock = new Mock<OutputWindowViewModel>(graphMock.Object, null);
			IoC.Get<IEventAggregator>().Subscribe(outputMock.Object);

			vm.Parent.OpenWindows.Clear();
			vm.Parent.OpenWindows.Add(outputMock.Object);
			vm.PlayPause();
			Thread.Sleep(2000);
			Assert.False(vm.IsPlaying);
			Assert.Equal(10, vm.Parent.Model.CurrentTick);
			outputMock.Verify(o => o.Handle(It.IsAny<TickRenderedMessage>()), Times.Exactly(10));

			vm.PlayPause();
			Thread.Sleep(1000);
			Assert.False(vm.IsPlaying);
			outputMock.Verify(o => o.Handle(It.IsAny<TickRenderedMessage>()), Times.Exactly(20));
		}

		[Fact]
		public void ICantBelieveImTestingThis()
		{
			vm.Parent.Model.Speed = 1;
			vm.Faster();
			Assert.True(vm.Parent.Model.Speed > 1);
			vm.Slower();
			Assert.Equal(1, vm.Parent.Model.Speed);
			vm.Slower();
			Assert.Equal(1, vm.Parent.Model.Speed);
		}
	}
}
