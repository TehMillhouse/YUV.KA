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
using YuvKA.Test.ViewModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Test
{
	public class CullInputTest
	{
		/// <summary>
		/// Checks whether inputs in a WeightedAveragedMergeNode are culled if an edge is deleted per drag-and-drop
		/// </summary>
		[Fact]
		public void WAMNDeleteEdge()
		{
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue(posMock.Object)
			).PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();

			// Add Nodes like so 
			// vin0
			//	   
			//		wamn
			//	   
			//vin1
			var vin0 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin1 = AddNodeModel<VideoInputNode>(vm, mock);
			var wamn = AddNodeModel<WeightedAveragedMergeNode>(vm, mock);

			// Add Edge like so
			// vin0
			//	   \
			//		wamn
			//	   
			//vin1
			// Start edge from vin0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over wamn input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on wamn input
			vm.InOutputMouseUp(wamn.Inputs.First());

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, wamn.Model.Inputs.Count());
			// WAMN-ViewModel has one fake input.
			Assert.Equal(2, wamn.Inputs.Count());


			// Add Edge like so
			// vin0
			//	   \
			//		wamn
			//	   /
			//vin1
			// Start edge from vin1 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin1.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over wamn input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on wamn input
			vm.InOutputMouseUp(wamn.Inputs.Last());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, wamn.Model.Inputs.Count());
			Assert.Equal(3, wamn.Inputs.Count());


			// Delete Edge like so
			// vin0
			//	   
			//		wamn
			//	   /
			//vin1
			// Pick up edge from wamn input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(wamn.Inputs.First())
			);

			edge = vm.DraggedEdge;

			// drag edge to empty space
			var mouseMock = new Mock<IMouseEventInfo>();
			mouseMock.Setup(m => m.GetPosition(vm)).Returns(new Point(50, 50));
			mouseMock.SetupGet(m => m.LeftButton).Returns(MouseButtonState.Pressed);
			vm.MouseMove(mouseMock.Object);

			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// let go
			vm.MouseUp();

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, wamn.Model.Inputs.Count());
			Assert.Equal(2, wamn.Inputs.Count());
		}

		/// <summary>
		/// Checks whether inputs in a WeightedAveragedMergeNode are culled if an edge is relocated to another node per drag-and-drop
		/// </summary>
		[Fact]
		public void WAMNRelocateEdge()
		{
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue(posMock.Object)
			).PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();

			// Add Nodes like so 
			// vin0   wamn0
			//	
			//	
			//	      wamn1
			//vin1
			var vin0 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin1 = AddNodeModel<VideoInputNode>(vm, mock);
			var wamn0 = AddNodeModel<WeightedAveragedMergeNode>(vm, mock);
			var wamn1 = AddNodeModel<WeightedAveragedMergeNode>(vm, mock);

			// Add Edge like so 
			// vin0---wamn0
			//	
			//	
			//	      wamn1
			//vin1
			// Start edge from vin0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over wamn0 input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn0.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on wamn0 input
			vm.InOutputMouseUp(wamn0.Inputs.First());

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, wamn0.Model.Inputs.Count());
			// WAMN-ViewModel has one fake input.
			Assert.Equal(2, wamn0.Inputs.Count());


			// Add Edge like so 
			// vin0---wamn0
			//	     /
			//	    /
			//	   /   wamn1
			//vin1
			// Start edge from vin1 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin1.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over wamn0 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn0.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on wamn0 input
			vm.InOutputMouseUp(wamn0.Inputs.Last());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, wamn0.Model.Inputs.Count());
			Assert.Equal(3, wamn0.Inputs.Count());
			Assert.Equal(0, wamn1.Model.Inputs.Count());
			Assert.Equal(1, wamn1.Inputs.Count());


			// Relocate Edge like so 
			// vin0   wamn0
			//	   \ /
			//	    X
			//	   / \
			//vin1    wamn1
			// Pick up edge from wamn0 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(wamn0.Inputs.First())
			);

			edge = vm.DraggedEdge;

			// Drag edge over wamn1 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn1.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on wamn1 input
			vm.InOutputMouseUp(wamn1.Inputs.First());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(1, wamn0.Model.Inputs.Count());
			Assert.Equal(2, wamn0.Inputs.Count());
			Assert.Equal(1, wamn1.Model.Inputs.Count());
			Assert.Equal(2, wamn1.Inputs.Count());
		}

		/// <summary>
		/// Checks whether inputs in a WeightedAveragedMergeNode are culled if a node is deleted.
		/// </summary>
		[Fact]
		public void WAMNDeleteNode()
		{
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue(posMock.Object)
			).PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();

			// Add Nodes like so 
			// vin0
			//	   
			//		wamn
			//	   
			//vin1
			var vin0 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin1 = AddNodeModel<VideoInputNode>(vm, mock);
			var wamn = AddNodeModel<WeightedAveragedMergeNode>(vm, mock);

			// Add Edge like so 
			// vin0
			//	   \
			//		wamn
			//	   
			//vin1
			// Start edge from vin0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over vin1 input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on vin1 input
			vm.InOutputMouseUp(wamn.Inputs.First());

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, wamn.Model.Inputs.Count());
			// WAMN-ViewModel has one fake input.
			Assert.Equal(2, wamn.Inputs.Count());

			// Add Edge like so 
			// vin0
			//	   \
			//		wamn
			//	   /
			//vin1
			// Start edge from vin1 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin1.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over wamn input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(wamn.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on wamn input
			vm.InOutputMouseUp(wamn.Inputs.Last());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, wamn.Model.Inputs.Count());
			Assert.Equal(3, wamn.Inputs.Count());

			// Remove Node like so 
			// 
			// 
			//		wamn
			//	   /
			//vin1
			Assert.PropertyChanged(vm, "Edges", vin0.RemoveNode);
			Assert.DoesNotContain(vin0, vm.Nodes);
			Assert.DoesNotContain(vin0.Model, vm.Parent.Model.Graph.Nodes);


			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, wamn.Model.Inputs.Count());
			Assert.Equal(2, wamn.Inputs.Count());
		}

		/// <summary>
		/// Checks whether inputs in a DiagramNode are culled if an edge is deleted per drag-and-drop. Also checks proper behavior of its graphs.
		/// </summary>
		[Fact]
		public void DiagramNodeDeleteEdge()
		{
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue(posMock.Object)
			).PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();

			// Add Nodes like so 
			// vin0
			//	   
			// vin1   dn
			//	   
			// vin2
			var vin0 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin1 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin2 = AddNodeModel<VideoInputNode>(vm, mock);
			var dn = AddNodeModel<DiagramNode>(vm, mock);
			
			// open OutputWindow of DiagramNode
			var dnvm = new DiagramViewModel(dn.Model);

			// Add Edge like so 
			// vin0
			//	   \
			//		\
			// vin1  dn
			//	   
			// vin2
			// Start edge from vin0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn input
			vm.InOutputMouseUp(dn.Inputs.First());

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, dn.Model.Inputs.Count());
			// DiagramNode has one fake input.
			Assert.Equal(2, dn.Inputs.Count());


			// Add Edge like so 
			// vin0
			//	   \
			//		\
			// vin1--dn
			//	   
			// vin2
			// Start edge from vin1 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin1.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn input
			vm.InOutputMouseUp(dn.Inputs.Last());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, dn.Model.Inputs.Count());
			Assert.Equal(3, dn.Inputs.Count());


			// Add PixelDiff-graph of vin1 and set reference to vin0
			Assert.Equal(4, vm.Nodes.Count());
			dnvm.Reference = dnvm.Videos.First();
			dnvm.ChosenVideo = dnvm.Videos.Last();
			dnvm.AddGraph();
			dnvm.Graphs.Single().CurrentType =
				dnvm.Graphs.Single().AvailableTypes.First();
			Assert.Equal(1, dnvm.Graphs.Count());
			Assert.NotNull(dnvm.Reference);


			// Add Edge like so 
			// vin0
			//	   \
			//		\
			// vin1--dn
			//		/
			//	   /
			// vin2
			// Start edge from vin2 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin2.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn input
			vm.InOutputMouseUp(dn.Inputs.Last());

			Assert.Equal(3, vm.Edges.Count());
			Assert.Equal(3, dn.Model.Inputs.Count());
			Assert.Equal(4, dn.Inputs.Count());

			// Add PixelDiff-graph of vin2
			dnvm.ChosenVideo = dnvm.Videos.Last();
			dnvm.AddGraph();
			dnvm.Graphs.Last().CurrentType =
				dnvm.Graphs.Last().AvailableTypes.First();
			Assert.Equal(2, dnvm.Graphs.Count());
			Assert.NotNull(dnvm.Reference);
			Assert.True(dnvm.Graphs.All(graph => graph.CurrentType != null));


			// Delete Edge like so 
			// vin0
			//	   \
			//		\
			// vin1--dn
			//
			//
			// vin2
			// Pick up edge from vin2 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(dn.Inputs.Last(inoutvm => !inoutvm.IsFake))
			);

			edge = vm.DraggedEdge;

			// drag edge to empty space
			var mouseMock = new Mock<IMouseEventInfo>();
			mouseMock.Setup(m => m.GetPosition(vm)).Returns(new Point(50, 50));
			mouseMock.SetupGet(m => m.LeftButton).Returns(MouseButtonState.Pressed);
			vm.MouseMove(mouseMock.Object);

			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// let go
			vm.MouseUp();

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, dn.Model.Inputs.Count());
			Assert.Equal(3, dn.Inputs.Count());
			Assert.Equal(4, vm.Nodes.Count());

			Assert.NotNull(dnvm.Reference);
			Assert.Equal(1, dnvm.Graphs.Count());


			// Delete Edge like so 
			// vin0
			//
			//
			// vin1--dn
			//
			//
			// vin2
			// Pick up edge from vin0 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(dn.Inputs.First())
			);

			edge = vm.DraggedEdge;

			// drag edge to empty space
			mouseMock = new Mock<IMouseEventInfo>();
			mouseMock.Setup(m => m.GetPosition(vm)).Returns(new Point(50, 50));
			mouseMock.SetupGet(m => m.LeftButton).Returns(MouseButtonState.Pressed);
			vm.MouseMove(mouseMock.Object);

			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// let go
			vm.MouseUp();

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, dn.Model.Inputs.Count());
			Assert.Equal(2, dn.Inputs.Count());
			Assert.Equal(4, vm.Nodes.Count());

			Assert.Null(dnvm.Reference);
			Assert.Equal(1, dnvm.Graphs.Count());
		}

		/// <summary>
		/// Checks whether inputs in a DiagramNode are culled if an edge is relocated to another DiagramNode per drag-and-drop.
		/// Also checks proper behavior of its graphs.
		/// </summary>
		[Fact]
		public void DiagramNodeRelocateEdge()
		{
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var vm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue(posMock.Object)
			).PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();

			// Add Nodes like so 
			// vin0  dn0
			//
			//
			// vin1
			//
			//
			// vin2  dn1
			var vin0 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin1 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin2 = AddNodeModel<VideoInputNode>(vm, mock);
			var dn0 = AddNodeModel<DiagramNode>(vm, mock);
			var dn1 = AddNodeModel<DiagramNode>(vm, mock);
			
			// Add outputWindow to first DiagramNode
			var dnvm = new DiagramViewModel(dn0.Model);

			// Add Edge like so 
			// vin0--dn0
			//
			//
			// vin1
			//
			//
			// vin2  dn1
			// Start edge from vin0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn0 input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn0.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn0 input
			vm.InOutputMouseUp(dn0.Inputs.First());

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, dn0.Model.Inputs.Count());
			// DiagramNode has one fake input.
			Assert.Equal(2, dn0.Inputs.Count());


			// Add Edge like so 
			// vin0--dn0
			//		/
			//	   /
			// vin1
			//
			//
			// vin2  dn1
			// Start edge from vin1 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin1.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn0 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn0.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn0 input
			vm.InOutputMouseUp(dn0.Inputs.Last());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, dn0.Model.Inputs.Count());
			Assert.Equal(3, dn0.Inputs.Count());
			Assert.Equal(0, dn1.Model.Inputs.Count());
			Assert.Equal(1, dn1.Inputs.Count());

			// Add PixelDiff-graph of vin1 and set reference to vin0
			Assert.Equal(5, vm.Nodes.Count());
			dnvm.Reference = dnvm.Videos.First();
			dnvm.ChosenVideo = dnvm.Videos.Last();
			dnvm.AddGraph();
			dnvm.Graphs.Single().CurrentType =
				dnvm.Graphs.Single().AvailableTypes.First();
			Assert.Equal(1, dnvm.Graphs.Count());
			Assert.NotNull(dnvm.Reference);


			// Add Edge like so 
			// vin0--dn0
			//		/ |
			//	   /  |
			// vin1  /
			//		/
			//	   /
			// vin2  dn1
			// Start edge from vin2 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin2.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn0 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn0.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn0 input
			vm.InOutputMouseUp(dn0.Inputs.Last());

			Assert.Equal(3, vm.Edges.Count());
			Assert.Equal(3, dn0.Model.Inputs.Count());
			Assert.Equal(4, dn0.Inputs.Count());

			// Add PixelDiff-graph of vin2
			dnvm.ChosenVideo = dnvm.Videos.Last();
			dnvm.AddGraph();
			dnvm.Graphs.Last().CurrentType =
				dnvm.Graphs.Last().AvailableTypes.First();
			Assert.Equal(2, dnvm.Graphs.Count());
			Assert.NotNull(dnvm.Reference);
			Assert.True(dnvm.Graphs.All(graph => graph.CurrentType != null));

			// Relocate Edge like so 
			// vin0  dn0
			//	   \/ |
			//	   /\ |
			// vin1  X
			//		/ |
			//	   /  |
			// vin2  dn1
			// Pick up edge from dn0 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(dn0.Inputs.Last(inoutvm => !inoutvm.IsFake))
			);

			edge = vm.DraggedEdge;

			// Drag edge over dn1 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn1.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn1 input
			vm.InOutputMouseUp(dn1.Inputs.Last());

			Assert.Equal(3, vm.Edges.Count());
			Assert.Equal(2, dn0.Model.Inputs.Count());
			Assert.Equal(3, dn0.Inputs.Count());
			Assert.Equal(1, dn1.Model.Inputs.Count());
			Assert.Equal(2, dn1.Inputs.Count());

			Assert.NotNull(dnvm.Reference);
			Assert.Equal(1, dnvm.Graphs.Count());

			// Relocate Edge like so 
			// vin0  dn0
			//	   \/
			//	   /\
			// vin1  \
			//		  |
			//		  |
			// vin2--dn1
			// Pick up edge from dn0 input
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(dn0.Inputs.First())
			);

			edge = vm.DraggedEdge;

			// Drag edge over dn1 input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn1.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn1 input
			vm.InOutputMouseUp(dn1.Inputs.Last());

			Assert.Equal(3, vm.Edges.Count());
			Assert.Equal(1, dn0.Model.Inputs.Count());
			Assert.Equal(2, dn0.Inputs.Count());
			Assert.Equal(2, dn1.Model.Inputs.Count());
			Assert.Equal(3, dn1.Inputs.Count());

			Assert.Null(dnvm.Reference);
			Assert.Equal(1, dnvm.Graphs.Count());
		}

		/// <summary>
		/// Checks whether inputs in a DiagramNode are culled if a node is deleted.
		/// Also checks proper behavior of its graphs.
		/// </summary>
		
		[Fact]
		public void DiagramNodeDeleteNode()
		{
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };
			var posMock = new Mock<IGetPosition>();
			posMock.Setup(p => p.GetElementSize(It.IsAny<IViewAware>())).Returns(new Size(4, 4));
			posMock.Setup(p => p.ViewLoaded(It.IsAny<IViewAware>())).Returns(Observable.Never<Unit>());

			var mvm = MainViewModelTest.GetInstance(
				container => container.ComposeExportedValue(posMock.Object)
			);

			var vm = mvm.PipelineViewModel;

			var mock = new Mock<IDragEventInfo>();

			// Add Nodes like so 
			// vin0
			//
			//
			// vin1  dn
			//
			//
			// vin2
			var vin0 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin1 = AddNodeModel<VideoInputNode>(vm, mock);
			var vin2 = AddNodeModel<VideoInputNode>(vm, mock);
			var dn = AddNodeModel<DiagramNode>(vm, mock);
			var dnvm = new DiagramViewModel(dn.Model);

			// Add Edge like so 
			// vin0
			//	   \
			//		\
			// vin1  dn
			//
			//
			// vin2
			// Start edge from vin0 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin0.Outputs.First())
			);

			var edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn input
			var e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn.Inputs.First(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn input
			vm.InOutputMouseUp(dn.Inputs.First());

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, dn.Model.Inputs.Count());
			// DiagramNode has one fake input.
			Assert.Equal(2, dn.Inputs.Count());


			// Add Edge like so 
			// vin0
			//	   \
			//		\
			// vin1--dn
			//
			//
			// vin2
			// Start edge from vin1 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin1.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn input
			vm.InOutputMouseUp(dn.Inputs.Last());

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, dn.Model.Inputs.Count());
			Assert.Equal(3, dn.Inputs.Count());


			// Add PixelDiff-graph of vin1 and set reference to vin0
			Assert.Equal(4, vm.Nodes.Count());
			dnvm.Reference = dnvm.Videos.First();
			dnvm.ChosenVideo = dnvm.Videos.Last();
			dnvm.AddGraph();
			dnvm.Graphs.Single().CurrentType =
				dnvm.Graphs.Single().AvailableTypes.First();
			Assert.Equal(1, dnvm.Graphs.Count());
			Assert.NotNull(dnvm.Reference);


			// Add Edge like so 
			// vin0
			//	   \
			//		\
			// vin1--dn
			//		/
			//	   /
			// vin2
			// Start edge from vin2 output
			Assert.PropertyChanged(vm, "DraggedEdge",
				() => vm.InOutputMouseDown(vin2.Outputs.First())
			);

			edge = vm.DraggedEdge;
			Assert.Equal(EdgeStatus.Indeterminate, edge.Status);

			// Drag edge over dn input
			e = new RoutedEventArgs(Mouse.MouseMoveEvent);
			vm.InOutputMouseMove(dn.Inputs.Last(), e);

			Assert.Equal(EdgeStatus.Valid, edge.Status);

			// Drop edge on dn input
			vm.InOutputMouseUp(dn.Inputs.Last());

			Assert.Equal(3, vm.Edges.Count());
			Assert.Equal(3, dn.Model.Inputs.Count());
			Assert.Equal(4, dn.Inputs.Count());

			// Add PixelDiff-graph of vin2
			dnvm.ChosenVideo = dnvm.Videos.Last();
			dnvm.AddGraph();
			dnvm.Graphs.Last().CurrentType =
				dnvm.Graphs.Last().AvailableTypes.First();
			Assert.Equal(2, dnvm.Graphs.Count());
			Assert.NotNull(dnvm.Reference);
			Assert.True(dnvm.Graphs.All(graph => graph.CurrentType != null));

			// Delete Node like so 
			// vin0
			//	   \
			//		\
			// vin1--dn
			//
			//
			//
			Assert.PropertyChanged(vm, "Edges", vin2.RemoveNode);

			Assert.DoesNotContain(vin2, vm.Nodes);
			Assert.DoesNotContain(vin2.Model, vm.Parent.Model.Graph.Nodes);

			Assert.Equal(2, vm.Edges.Count());
			Assert.Equal(2, dn.Model.Inputs.Count());
			Assert.Equal(3, dn.Inputs.Count());
			Assert.Equal(3, vm.Nodes.Count());

			Assert.NotNull(dnvm.Reference);
			Assert.Equal(1, dnvm.Graphs.Count());

			// Delete Node like so 
			//
			//
			//
			// vin1--dn
			//
			//
			//
			Assert.PropertyChanged(vm, "Edges", vin0.RemoveNode);

			Assert.DoesNotContain(vin0, vm.Nodes);
			Assert.DoesNotContain(vin0.Model, vm.Parent.Model.Graph.Nodes);

			Assert.Equal(1, vm.Edges.Count());
			Assert.Equal(1, dn.Model.Inputs.Count());
			Assert.Equal(2, dn.Inputs.Count());
			Assert.Equal(2, vm.Nodes.Count());

			Assert.Null(dnvm.Reference);
			Assert.Equal(1, dnvm.Graphs.Count());
		}

		private static NodeViewModel AddNodeModel<T>(PipelineViewModel pvm, Mock<IDragEventInfo> mock)
		{
			mock.Setup(e => e.GetData<NodeType>()).Returns(new NodeType { Type = typeof(T) });
			mock.Setup(e => e.GetPosition(pvm)).Returns(new Point(50, 70));
			mock.SetupProperty(e => e.Effects, DragDropEffects.Copy);

			pvm.CheckClearance(mock.Object);
			Assert.Equal(DragDropEffects.Copy, mock.Object.Effects);

			pvm.Drop(mock.Object);

			var node = pvm.Nodes.Last();
			Assert.True(node.Model is T);
			Assert.Equal(50, node.Model.X);
			Assert.Equal(70, node.Model.Y);

			return node;
		}
	}
}
