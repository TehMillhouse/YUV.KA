using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.ViewModel;
using YuvKA.ViewModel;

namespace YuvKA.Test
{
	public class GlobalTestT10
	{
		[Fact]
		public void GlobalTest10()
		{
			MainViewModel mvm = MainViewModelTest.GetInstance();
			PipelineViewModel pvm = mvm.PipelineViewModel;

			// Step 1: The user clicks "New" to create a new pipeline
			mvm.Clear();
			Assert.Empty(mvm.Model.Graph.Nodes);
			var mock = new Mock<IDragEventInfo>();

			// Step 2: Create each type of node once by drag-and-drop
			VideoInputNode vin = (VideoInputNode)AddNode<VideoInputNode>(pvm, mock, new Point(10, 30));
			AdditiveMergeNode amn = (AdditiveMergeNode)AddNode<AdditiveMergeNode>(pvm, mock, new Point(30, 30));
			BlurNode bn = (BlurNode)AddNode<BlurNode>(pvm, mock, new Point(50, 30));
			BrightnessContrastSaturationNode bcsn =
				(BrightnessContrastSaturationNode)AddNode<BrightnessContrastSaturationNode>(pvm, mock, new Point(70, 30));
			ColorInputNode cin = (ColorInputNode)AddNode<ColorInputNode>(pvm, mock, new Point(10, 50));
			DelayNode dln = (DelayNode)AddNode<DelayNode>(pvm, mock, new Point(90, 30));
			DiagramNode dgn = (DiagramNode)AddNode<DiagramNode>(pvm, mock, new Point(110, 30));
			DifferenceNode dfn = (DifferenceNode)AddNode<DifferenceNode>(pvm, mock, new Point(30, 50));
			HistogramNode hn = (HistogramNode)AddNode<HistogramNode>(pvm, mock, new Point(50, 50));
			ImageInputNode imin = (ImageInputNode)AddNode<ImageInputNode>(pvm, mock, new Point(70, 50));
			InverterNode invn = (InverterNode)AddNode<InverterNode>(pvm, mock, new Point(90, 50));
			NoiseInputNode nin = (NoiseInputNode)AddNode<NoiseInputNode>(pvm, mock, new Point(110, 50));
			OverlayNode on = (OverlayNode)AddNode<OverlayNode>(pvm, mock, new Point(10, 70));
			RgbSplitNode rgbsn = (RgbSplitNode)AddNode<RgbSplitNode>(pvm, mock, new Point(30, 70));
			WeightedAveragedMergeNode wamn =
				(WeightedAveragedMergeNode)AddNode<WeightedAveragedMergeNode>(pvm, mock, new Point(50, 70));

			// Step 3: Create the edges
			mvm.Model.Graph.AddEdge(vin.Outputs[0], bn.Inputs[0]);
			Assert.Equal(vin.Outputs[0], bn.Inputs[0].Source);
			amn.Inputs.Add(new Node.Input());
			mvm.Model.Graph.AddEdge(vin.Outputs[0], amn.Inputs[0]);
			Assert.Equal(vin.Outputs[0], amn.Inputs[0].Source);
			mvm.Model.Graph.AddEdge(bn.Outputs[0], dln.Inputs[0]);
			Assert.Equal(bn.Outputs[0], dln.Inputs[0].Source);
			mvm.Model.Graph.AddEdge(dln.Outputs[0], dfn.Inputs[0]);
			Assert.Equal(dln.Outputs[0], dfn.Inputs[0].Source);
			mvm.Model.Graph.AddEdge(imin.Outputs[0], dfn.Inputs[1]);
			Assert.Equal(imin.Outputs[0], dfn.Inputs[1].Source);
			mvm.Model.Graph.AddEdge(dfn.Outputs[0], invn.Inputs[0]);
			Assert.Equal(dfn.Outputs[0], invn.Inputs[0].Source);
			mvm.Model.Graph.AddEdge(invn.Outputs[0], on.Inputs[0]);
			Assert.Equal(invn.Outputs[0], on.Inputs[0].Source);
			mvm.Model.Graph.AddEdge(vin.Outputs[0], on.Inputs[1]);
			Assert.Equal(vin.Outputs[0], on.Inputs[1].Source);
			mvm.Model.Graph.AddEdge(vin.Outputs[0], rgbsn.Inputs[0]);
			Assert.Equal(vin.Outputs[0], rgbsn.Inputs[0].Source);
			mvm.Model.Graph.AddEdge(rgbsn.Outputs[2], hn.Inputs[0]);
			Assert.Equal(rgbsn.Outputs[2], hn.Inputs[0].Source);
		}

		private Node AddNode<T>(PipelineViewModel pvm, Mock<IDragEventInfo> mock, Point p)
		{
			mock.Setup(e => e.GetData<NodeType>()).Returns(new NodeType { Type = typeof(T) });
			mock.Setup(e => e.GetPosition(pvm)).Returns(new Point(50, 70));
			mock.SetupProperty(e => e.Effects, DragDropEffects.Copy);

			pvm.CheckClearance(mock.Object);
			Assert.Equal(DragDropEffects.Copy, mock.Object.Effects);

			pvm.Drop(mock.Object);

			var node = pvm.Nodes.Last().Model;
			Assert.True(node is T);
			Assert.Equal(50, node.X);
			Assert.Equal(70, node.Y);

			return node;
		}
	}
}
