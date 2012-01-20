﻿using System;
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
					select new EdgeViewModel(this) { StartViewModel = input, EndViewModel = output };

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

		public void Drop(DragEventArgs e)
		{
			var type = (NodeType)e.Data.GetData(typeof(NodeType));
			var node = (Node)Activator.CreateInstance(type.Type);
			var nodeModel = new NodeViewModel(node, this);
			nodeModel.Position = IoC.Get<IGetPosition>().GetDropPosition(e, this);

			Parent.Model.Graph.Nodes.Add(node);
			Nodes.Add(nodeModel);
		}

		public void NodeMouseDown(NodeViewModel node, MouseEventArgs e)
		{
			draggedNode = node;
			dragMouseOffset = IoC.Get<IGetPosition>().GetMousePosition(e, this) - draggedNode.Position;
		}

		public void InOutputMouseDown(InOutputViewModel inOut)
		{
			InOutputViewModel start = inOut;
			// If the input is already connected, drag the existing edge
			if (inOut.Model is Node.Input) {
				start = GetOutputViewModel(((Node.Input)inOut.Model).Source) ?? start;
				((Node.Input)inOut.Model).Source = null;
				NotifyOfPropertyChange(() => Edges);
			}
			DraggedEdge = new EdgeViewModel(this) { StartViewModel = start, EndViewModel = inOut };
		}

		public void MouseMove(MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) {
				draggedNode = null;
				DraggedEdge = null;
				return;
			}

			IGetPosition getPos = IoC.Get<IGetPosition>();
			if (draggedNode != null)
				draggedNode.Position = getPos.GetMousePosition(e, this) - dragMouseOffset;
			else if (DraggedEdge != null)
				DraggedEdge.EndPoint = getPos.GetMousePosition(e, this);
		}

		public void MouseUp()
		{
			draggedNode = null;
			DraggedEdge = null;
		}

		public void InOutputMouseUp(InOutputViewModel inOut)
		{
			InOutputViewModel inputVM, outputVM;
			if ((DraggedEdge.StartViewModel.IsFake || DraggedEdge.StartViewModel.Model is Node.Input) && inOut.Model is Node.Output) {
				inputVM = DraggedEdge.StartViewModel;
				outputVM = inOut;
			}
			else if (DraggedEdge.StartViewModel.Model is Node.Output && (inOut.IsFake || inOut.Model is Node.Input)) {
				inputVM = inOut;
				outputVM = DraggedEdge.StartViewModel;
			}
			else {
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

			if (Parent.Model.Graph.AddEdge(output, input))
				NotifyOfPropertyChange(() => Edges);
			DraggedEdge = null;
		}

		InOutputViewModel GetOutputViewModel(Node.Output output)
		{
			return output == null ? null : Nodes.SelectMany(n => n.Outputs).Single(o => o.Model == output);
		}
	}
}
