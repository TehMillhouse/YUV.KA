using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class PipelineViewModel : ViewAware
	{
		NodeViewModel draggedNode;
		EdgeViewModel draggedEdge;
		Vector dragMouseOffset;
		IEnumerable<EdgeViewModel> edges = Enumerable.Empty<EdgeViewModel>();
		int maxZValue;

		public PipelineViewModel(MainViewModel parent)
		{
			Parent = parent;
			Nodes = new ObservableCollection<NodeViewModel>(Parent.Model.Graph.Nodes.Select(n => new NodeViewModel(n, this)));
		}

		public MainViewModel Parent { get; private set; }
		public IList<NodeViewModel> Nodes { get; private set; }
		public IEnumerable<EdgeViewModel> Edges
		{
			get
			{
				foreach (EdgeViewModel edge in edges)
					edge.Dispose();

				// Linq query... OF DEATH
				var newEdges =
					from node in Nodes
					from input in node.Inputs
					where !input.IsFake
					let iModel = ((Node.Input)input.Model)
					where iModel.Source != null
					let output = GetOutputViewModel(iModel.Source)
					select new EdgeViewModel { StartViewModel = input, EndViewModel = output };

				return edges = newEdges.ToList();
			}
		}

		public EdgeViewModel DraggedEdge
		{
			get { return draggedEdge; }
			set
			{
				draggedEdge = value;
				NotifyOfPropertyChange(() => DraggedEdge);
			}
		}

		public void Drop(IDragEventInfo e)
		{
			/* Only allow this if pipline is not rendering */
			if (!Parent.ReplayStateViewModel.IsPlaying) {
				var type = e.GetData<NodeType>();
				var node = (Node)Activator.CreateInstance(type.Type);
				var nodeModel = new NodeViewModel(node, this);
				nodeModel.Position = e.GetPosition(relativeTo: this);
				nodeModel.ZIndex = maxZValue++;

				Parent.Model.Graph.AddNode(node);
				Nodes.Add(nodeModel);
				Parent.SaveSnapshot();
			}
		}

		public void NodeMouseDown(NodeViewModel node, IMouseEventInfo e)
		{
			draggedNode = node;
			dragMouseOffset = e.GetPosition(relativeTo: this) - draggedNode.Position;

			draggedNode.ZIndex = maxZValue++;
			draggedNode.NotifyOfPropertyChange(() => draggedNode.ZIndex);
		}

		public void InOutputMouseDown(InOutputViewModel inOut)
		{
			/* Only allow this if pipline is not rendering */
			if (!Parent.ReplayStateViewModel.IsPlaying) {
				InOutputViewModel start = inOut;
				// If the input is already connected, drag the existing edge
				if (inOut.Model is Node.Input) {
					start = GetOutputViewModel(((Node.Input)inOut.Model).Source) ?? start;
					((Node.Input)inOut.Model).Source = null;
					NotifyOfPropertyChange(() => Edges);
				}
				DraggedEdge = new EdgeViewModel { StartViewModel = start, EndViewModel = inOut };
			}
		}

		public void MouseMove(IMouseEventInfo e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) {
				draggedNode = null;
				DraggedEdge = null;
				return;
			}

			IGetPosition getPos = IoC.Get<IGetPosition>();
			if (draggedNode != null)
				draggedNode.Position = e.GetPosition(relativeTo: this) - dragMouseOffset;
			else if (DraggedEdge != null) {
				if (DraggedEdge.Status != EdgeStatus.Indeterminate)
					DraggedEdge.Status = EdgeStatus.Indeterminate;
				DraggedEdge.EndPoint = e.GetPosition(relativeTo: this);
			}
		}

		public void InOutputMouseMove(InOutputViewModel inOut, RoutedEventArgs e)
		{
			if (DraggedEdge == null)
				return;

			InOutputViewModel inputVM, outputVM;
			DraggedEdge.EndViewModel = inOut;
			DraggedEdge.Status =
				DraggedEdge.GetInOut(out inputVM, out outputVM) && Parent.Model.Graph.CanAddEdge(outputVM.Parent.Model, inputVM.Parent.Model) ?
				EdgeStatus.Valid : EdgeStatus.Invalid;
			e.Handled = true; // don't bubble up into MouseMove
		}

		public void MouseUp()
		{
			draggedNode = null;
			DraggedEdge = null;
		}

		public void InOutputMouseUp(InOutputViewModel inOut)
		{
			if (DraggedEdge == null)
				return;

			DraggedEdge.EndViewModel = inOut;
			InOutputViewModel inputVM, outputVM;
			if (!DraggedEdge.GetInOut(out inputVM, out outputVM)) {
				DraggedEdge = null;
				return;
			}
			DraggedEdge = null;

			var output = (Node.Output)outputVM.Model;
			Node.Input input;
			if (inputVM.IsFake) {
				// realize substitute input
				if (Parent.Model.Graph.CanAddEdge(output.Node, inputVM.Parent.Model))
					input = inputVM.Parent.AddInput();
				else
					return;
			}
			else
				input = (Node.Input)inputVM.Model;

			if (Parent.Model.Graph.AddEdge(output, input)) {
				NotifyOfPropertyChange(() => Edges);
				Parent.SaveSnapshot();
			}
			DraggedEdge = null;
		}

		public void CheckClearance(DragEventArgs e)
		{
			if (Parent.ReplayStateViewModel.IsPlaying) {
				e.Effects = DragDropEffects.None;
			}
		}

		InOutputViewModel GetOutputViewModel(Node.Output output)
		{
			return output == null ? null : Nodes.SelectMany(n => n.Outputs).Single(o => o.Model == output);
		}
	}
}
