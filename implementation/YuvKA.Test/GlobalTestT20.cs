using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.ViewModel;
using YuvKA.ViewModel;

namespace YuvKA.Test
{
	public class GlobalTestT20
	{
		[Fact]
		public void GlobalTestCaseT20()
		{
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue<IGetPosition>(posMock.Object)
			).PipelineViewModel;

			// Step 1: User creates a random pipeline with three nodes and one edge connecting them.
			// Add BlurNode
			var mock = new Mock<IDragEventInfo>();
			mock.Setup(info => info.GetData<NodeType>()).Returns(new NodeType { Type = typeof(BlurNode) });
			mock.Setup(info => info.GetPosition(vm)).Returns(new Point(30, 10));
			mock.SetupProperty(info => info.Effects, DragDropEffects.Copy);

			vm.CheckClearance(mock.Object);
			Assert.Equal(DragDropEffects.Copy, mock.Object.Effects);

			vm.Drop(mock.Object);

			var node0 = vm.Nodes.Single();
			Assert.True(node0.Model is BlurNode);
			Assert.Equal(30, node0.Model.X);
			Assert.Equal(10, node0.Model.Y);

			//Add Second BlurNode
			mock = new Mock<IDragEventInfo>();
			mock.Setup(info => info.GetData<NodeType>()).Returns(new NodeType { Type = typeof(BlurNode) });
			mock.Setup(info => info.GetPosition(vm)).Returns(new Point(40, 10));
			mock.SetupProperty(info => info.Effects, DragDropEffects.Copy);

			vm.CheckClearance(mock.Object);
			Assert.Equal(DragDropEffects.Copy, mock.Object.Effects);

			vm.Drop(mock.Object);

			var node1 = vm.Nodes.Last();
			Assert.True(node1.Model is BlurNode);
			Assert.Equal(40, node1.Model.X);
			Assert.Equal(10, node1.Model.Y);

			//Add third BlurNode
			mock = new Mock<IDragEventInfo>();
			mock.Setup(info => info.GetData<NodeType>()).Returns(new NodeType { Type = typeof(BlurNode) });
			mock.Setup(info => info.GetPosition(vm)).Returns(new Point(0, 10));
			mock.SetupProperty(info => info.Effects, DragDropEffects.Copy);

			vm.CheckClearance(mock.Object);
			Assert.Equal(DragDropEffects.Copy, mock.Object.Effects);

			vm.Drop(mock.Object);

			var node2 = vm.Nodes.Last();
			Assert.True(node1.Model is BlurNode);
			Assert.Equal(0, node2.Model.X);
			Assert.Equal(10, node2.Model.Y);

			//Add Edge
			
			posMock.Setup(p => p.GetElementPosition(node0.Outputs.Single(), vm)).Returns(new Point(10, 10));
			posMock.Setup(p => p.GetElementPosition(node0.Inputs.Single(), vm)).Returns(new Point(30, 10));
			posMock.Setup(p => p.GetElementPosition(node1.Inputs.Single(), vm)).Returns(new Point(40, 10));
			posMock.Setup(p => p.GetElementPosition(node2.Inputs.Single(), vm)).Returns(new Point(0, 10));

			// Start edge from node0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(node0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(new Point(12, 12), edge.StartPoint);
			Assert.Equal(new Point(12, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over node1 input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(node1.Inputs.Single(), e);

			Assert.Equal(new Point(42, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on node1 input

			vm.InOutputMouseUp(node1.Inputs.Single());

			Assert.Equal(new Point(42, 12), vm.Edges.Single().StartPoint);
			Assert.Equal(new Point(12, 12), vm.Edges.Single().EndPoint);
			Assert.NotNull(vm.Edges.Single().Geometry);

			// Step 2: User moves a node via drag-and-drop

			posMock.Setup(p => p.GetElementPosition(node0.Outputs.Single(), vm)).Returns(new Point(20, 10));
			Assert.PropertyChanged(edge, "Geometry",
				() => node0.Position = new Point(-1, -1) // Trigger ViewPositionChanged
			);
			Assert.Equal(new Point(-1, -1), node0.Position);

			// Step 3: User changes the end of an edge via drag-and-drop

			// Pick up edge from node1 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(node1.Inputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(new Point(22, 12), edge.StartPoint);
			Assert.Equal(new Point(42, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over node2 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(node2.Inputs.Single(), e);

			Assert.Equal(new Point(2, 12), edge.EndPoint);
			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on node2 input

			vm.InOutputMouseUp(node2.Inputs.Single());

			Assert.Equal(new Point(2, 12), vm.Edges.Single().StartPoint);
			Assert.Equal(new Point(22, 12), vm.Edges.Single().EndPoint);
			Assert.NotNull(vm.Edges.Single().Geometry);

			// Step 4: user removes an edge

			// Pick up edge from node2 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(node2.Inputs.First())
			);
			
			edge = vm.DraggedEdge;
			Assert.Equal(new Point(22, 12), edge.StartPoint);
			Assert.Equal(new Point(2, 12), edge.EndPoint);

			// drag edge to empty space
			var mouseMock = new Mock<IMouseEventInfo>();
			mouseMock.Setup(m => m.GetPosition(vm)).Returns(new Point(50, 50));
			mouseMock.SetupGet(m => m.LeftButton).Returns(MouseButtonState.Pressed);
			vm.MouseMove(mouseMock.Object);

			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);
			Assert.Equal(new Point(50, 50), edge.EndPoint);

			// let go
			vm.MouseUp();

			Assert.Equal(0, vm.Edges.Count());

			// Step 5: user changes the configuration of a manipulation-node
			((BlurNode) node0.Model).Radius = 2;

			// Step 6: user deletes a node
			Assert.PropertyChanged(vm, "Edges", node0.RemoveNode);

			Assert.DoesNotContain(node0, vm.Nodes);
			Assert.DoesNotContain(node0.Model, vm.Parent.Model.Graph.Nodes);
		}
	}
}
