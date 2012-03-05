using System.Linq;
using Xunit;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Test.ViewModel
{
	public class HistogramViewModelTest
	{
		/// <summary>
		/// Checks whether the calculated data is correctly relayed to the HistogramVM.
		/// </summary>
		[Fact]
		public void GetsData()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new HistogramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var hvm = new HistogramViewModel((HistogramNode)vm.Nodes.Single().Model);

			// Generates an array of 1 Frame with randomly filled Data
			var testSize = new Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (var x = 0; x < testSize.Width; x++) {
				for (var y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}

			hvm.NodeModel.Type = HistogramType.R;

			// porcess one frame with chosen type
			hvm.NodeModel.Process(inputs, 0);
			hvm.Handle(null);

			Assert.NotEmpty(hvm.Data.Data);
		}
	}
}
