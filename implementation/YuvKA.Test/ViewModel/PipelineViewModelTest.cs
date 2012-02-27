using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA.Pipeline;
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

		[Fact]
		public void CanAddInput()
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
			var node1 = new NodeViewModel(new WeightedAveragedMergeNode(), vm);
			vm.Nodes.Add(node1);
			vm.Parent.Model.Graph.AddNode(node1.Model);

			Assert.Equal(0, node1.Model.Inputs.Count);
			Assert.Equal(1, node1.Inputs.Count());

			vm.InOutputMouseDown(node0.Outputs.First());
			vm.InOutputMouseUp(node1.Inputs.Last());

			Assert.Equal(1, node1.Model.Inputs.Count);
			Assert.Equal(2, node1.Inputs.Count());
			Assert.Equal(node1.Inputs.First(), vm.Edges.Single().StartViewModel);
		}

		[Fact]
		public void CanRemoveNode()
		{
			PipelineViewModel vm = MainViewModelTest.GetInstance().PipelineViewModel;

			var node0 = new NodeViewModel(new BlurNode(), vm);
			vm.Nodes.Add(node0);
			vm.Parent.Model.Graph.AddNode(node0.Model);

			Assert.PropertyChanged(vm, "Edges", node0.RemoveNode);

			Assert.DoesNotContain(node0, vm.Nodes);
			Assert.DoesNotContain(node0.Model, vm.Parent.Model.Graph.Nodes);
		}

		[Fact]
		public void NotifiesOfViewPositionChanges()
		{
			var obs = new Mock<IObserver<Unit>>();
			var node = new NodeViewModel(new BlurNode(), null);
			node.ViewPositionChanged.Subscribe(obs.Object);

			node.ViewLoaded();
			obs.Verify(o => o.OnNext(Unit.Default));

			node.Position = new Point(123, 123);
			obs.Verify(o => o.OnNext(Unit.Default), Times.Exactly(2));
		}

		[Fact]
		public void RendersNodeToFileCorrectly()
		{
			string input = @"..\..\..\..\resources\americanFootball_352x240_125.yuv";
			var windowManMock = new Mock<IWindowManager>();
			var conductorMock = new Mock<IConductor>();
			windowManMock.Setup(w => w.ShowDialog(It.IsAny<IScreen>(), null, null)).Callback<object, object, object>((viewModel, _, __) => {
				var screen = (Screen)viewModel;
				screen.Parent = conductorMock.Object;
				((IActivate)screen).Activate();
				while (screen.IsActive)
					Thread.Sleep(100);
			});
			conductorMock.Setup(c => c.DeactivateItem(It.IsAny<IScreen>(), true))
				.Callback<object, bool>((window, _) => ((IDeactivate)window).Deactivate(close: true))
				.Verifiable();
			var vm = MainViewModelTest.GetInstance(cont => cont.ComposeExportedValue<IWindowManager>(windowManMock.Object)).PipelineViewModel;

			var node = new NodeViewModel(new VideoInputNode { FileName = new FilePath(input) }, vm);
			vm.Parent.Model.Graph.AddNode(node.Model);
			var stream = new MemoryStream();

			IEnumerator<IResult> result = node.SaveNodeOutput(node.Model.Outputs[0]).GetEnumerator();
			result.MoveNext();
			var file = (ChooseFileResult)result.Current;
			Assert.Equal(false, file.OpenReadOnly);
			file.Stream = () => stream;
			result.MoveNext();

			Assert.Equal(File.ReadAllBytes(input).Length, stream.ToArray().Length);
		}

		[Fact]
		public void DisplaysDynamicallyAddedOutputs()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;

			var nodeMock = new Mock<Node>(new object[] { 0, null }); // input count, output count
			var nodeVM = new NodeViewModel(nodeMock.Object, vm);
			Assert.False(nodeVM.HasOutputs);
			Assert.Empty(nodeVM.Outputs);

			nodeMock.Object.Outputs.Add(new Node.Output(nodeMock.Object));
			Assert.True(nodeVM.HasOutputs);
			Assert.Equal(nodeMock.Object.Outputs[0], nodeVM.Outputs.Single().Model);
		}
	}
}
