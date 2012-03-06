using System;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.ViewModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Test
{
	public class GlobalTestT40
	{
		/// <summary>
		/// Creates a simple pipeline and plays around with the functionality a bit, playing, pausing, etc.
		/// pipeline:
		/// [video]--->[blur]--->[display]
		/// </summary>
		[Fact]
		public void Test40()
		{
			// Step 1: create an arbitrary acyclical pipeline with an input and a manipulation node
			MainViewModel mvm = MainViewModelTest.GetInstance();
			VideoInputNode video = new VideoInputNode();
			BlurNode blur = new BlurNode() { Type = BlurType.Gaussian, Radius = 3 };
			DisplayNode display = new DisplayNode();
			// We'll have to dump this node's output later, so we need a ViewModel
			NodeViewModel blurVM = new NodeViewModel(blur, mvm.PipelineViewModel);

			mvm.Model.Graph.AddNode(video);
			mvm.Model.Graph.AddNode(blur);
			mvm.Model.Graph.AddNode(display);
			mvm.Model.Graph.AddEdge(video.Outputs[0], blur.Inputs[0]);
			mvm.Model.Graph.AddEdge(blur.Outputs[0], display.Inputs[0]);
			// Assert that the graph is well-formed
			Assert.Contains(video, mvm.Model.Graph.Nodes);
			Assert.Contains(blur, mvm.Model.Graph.Nodes);
			Assert.Contains(display, mvm.Model.Graph.Nodes);
			Assert.Equal(blur.Inputs[0].Source, video.Outputs[0]);
			Assert.Equal(display.Inputs[0].Source, blur.Outputs[0]);

			// Step 2: change the video input data
			Assert.False(display.InputIsValid);
			video.FileName = new FilePath(@"..\..\..\..\resources\americanFootball_352x240_125.yuv");
			Assert.True(display.InputIsValid);

			// Step 3: open the DisplayNode's output and play the video
			DisplayViewModel output = display.Window;
			Assert.Equal(display, output.NodeModel);

			Assert.False(mvm.ReplayStateViewModel.IsPlaying);
			mvm.ReplayStateViewModel.PlayPause();
			Assert.True(mvm.ReplayStateViewModel.IsPlaying);

			// Step 4: Change a node's options while the video is playing
			blur.Radius = 0;
			Assert.True(mvm.ReplayStateViewModel.IsPlaying);
			Assert.True(display.InputIsValid);

			// Step 5: change replay speed while video is playing
			int oldSpeed = mvm.Model.Speed;
			mvm.ReplayStateViewModel.Slower();
			Assert.True(oldSpeed > mvm.Model.Speed);

			// Step 6: pause the video
			mvm.ReplayStateViewModel.PlayPause();
			Assert.False(mvm.ReplayStateViewModel.IsPlaying);

			// Step 7: resumes the video
			mvm.ReplayStateViewModel.PlayPause();
			Assert.True(mvm.ReplayStateViewModel.IsPlaying);

			// Step 8: resets the video playing state
			mvm.ReplayStateViewModel.Stop();
			Assert.Equal(0, mvm.Model.CurrentTick);
			Assert.False(mvm.ReplayStateViewModel.IsPlaying);

			// Step 9: save the video as yuv file
			blurVM.SaveNodeOutput(blur.Outputs[0]);
			// Since we can't create a FileChooser here, we'll have to invoke the SaveNodeOutputViewModel directly
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			SaveNodeOutputViewModel saveVM = new SaveNodeOutputViewModel(blur.Outputs[0], stream, mvm.Model);
			Assert.Equal(mvm.Model.Graph.TickCount, saveVM.TickCount);
			// I'm sorry for this. I really am. I found no better way of testing this.
			System.Threading.Thread.Sleep(1000);
			try {
				Assert.NotEqual(0, stream.Capacity);
			} catch (ObjectDisposedException) {
				// This means the SaveNodeOutputViewModel is already done and has disposed of its stream
				// That's just another possibility that's just as valid, and signifies proper execution
			}
		}
	}
}