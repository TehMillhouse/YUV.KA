using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA.Pipeline.Implementation;
using YuvKA.ViewModel;

namespace YuvKA.Test.ViewModel
{
	public class PipelineViewModelTest
	{
		[Fact]
		public void CanDropNode()
		{
			PipelineViewModel vm = MainViewModelTest.GetInstance().PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();
			mock.Setup(e => e.GetData<NodeType>()).Returns(new NodeType { Type = typeof(BlurNode) });
			mock.Setup(e => e.GetPosition(vm)).Returns(new Point(42, 21));
			mock.SetupProperty(e => e.Effects, DragDropEffects.Copy);

			vm.CheckClearance(mock.Object);
			Assert.Equal(DragDropEffects.Copy, mock.Object.Effects);

			vm.Drop(mock.Object);

			var node = vm.Nodes.Single().Model;
			Assert.True(node is BlurNode);
			Assert.Equal(42, node.X);
			Assert.Equal(21, node.Y);
		}

		[Fact]
		public void CanMoveNode()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;

			var node0 = new NodeViewModel(new BlurNode(), vm);
			vm.Nodes.Add(node0);
			vm.Parent.Model.Graph.AddNode(node0.Model);

			var mock = new Mock<IMouseEventInfo>();
			mock.Setup(e => e.GetPosition(vm)).Returns(new Point(5, 4));
			vm.NodeMouseDown(node0, mock.Object);

			mock.Setup(e => e.GetPosition(vm)).Returns(new Point(25, 14));
			mock.SetupGet(e => e.LeftButton).Returns(MouseButtonState.Pressed);
			vm.MouseMove(mock.Object);
			vm.MouseUp();

			Assert.Equal(new Point(20, 10), node0.Position);
		}

		[Fact]
		public void CanDragEdge()
		{
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			PipelineViewModel vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue<IGetPosition>(posMock.Object)
			).PipelineViewModel;

			var node0 = new NodeViewModel(new BlurNode(), vm);
			vm.Nodes.Add(node0);
			vm.Parent.Model.Graph.AddNode(node0.Model);
			var node1 = new NodeViewModel(new BlurNode(), vm);
			vm.Nodes.Add(node1);
			vm.Parent.Model.Graph.AddNode(node1.Model);

			posMock.Setup(p => p.GetElementPosition(node0.Outputs.Single(), vm)).Returns(new Point(10, 10));
			posMock.Setup(p => p.GetElementPosition(node0.Inputs.Single(), vm)).Returns(new Point(30, 10));
			posMock.Setup(p => p.GetElementPosition(node1.Inputs.Single(), vm)).Returns(new Point(40, 10));

			// Start edge from node0 output

			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(node0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(new Point(12, 12), edge.StartPoint);
			Assert.Equal(new Point(12, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Move node0

			posMock.Setup(p => p.GetElementPosition(node0.Outputs.Single(), vm)).Returns(new Point(20, 10));
			Assert.PropertyChanged(edge, "Geometry",
				() => node0.Position = new Point(-1, -1) // Trigger ViewPositionChanged
			);
			Assert.Equal(new Point(22, 12), edge.StartPoint);
			Assert.Equal(new Point(22, 12), edge.EndPoint);

			// Drag edge

			var mouseMock = new Mock<IMouseEventInfo>();
			mouseMock.Setup(m => m.GetPosition(vm)).Returns(new Point(50, 50));
			mouseMock.SetupGet(m => m.LeftButton).Returns(MouseButtonState.Pressed);
			vm.MouseMove(mouseMock.Object);

			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);
			Assert.Equal(new Point(50, 50), edge.EndPoint);

			// Drag edge over node0 input

			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(node0.Inputs.Single(), e);

			Assert.Equal(new Point(32, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Invalid, edge.Status);

			// Drag edge over node1 input

			vm.InOutputMouseMove(node1.Inputs.Single(), e);

			Assert.Equal(new Point(42, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on node1 input

			vm.InOutputMouseUp(node1.Inputs.Single());

			Assert.Equal(new Point(42, 12), vm.Edges.Single().StartPoint);
			Assert.Equal(new Point(22, 12), vm.Edges.Single().EndPoint);
			Assert.NotNull(vm.Edges.Single().Geometry);
		}
	}
}
