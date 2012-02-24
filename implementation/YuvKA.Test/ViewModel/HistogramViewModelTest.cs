using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Test.ViewModel
{
	public class HistogramViewModelTest
	{
		[Fact]
		public void GetsData()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new HistogramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var HVM = new HistogramViewModel((HistogramNode)vm.Nodes.Single().Model);

			// Generates an array of 1 Frame with randomly filled Data
			var testSize = new Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (var x = 0; x < testSize.Width; x++) {
				for (var y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}

			HVM.NodeModel.Type = HistogramType.R;

			HVM.NodeModel.Process(inputs, 0);

			HVM.Handle(null);

			Assert.NotEmpty(HVM.Data.Data);
		}
	}
}
