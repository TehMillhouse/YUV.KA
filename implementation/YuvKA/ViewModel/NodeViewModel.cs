using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// View model for a Node instance
	/// </summary>
	public class NodeViewModel : ViewAware
	{
		InOutputViewModel fake;
		IList<InOutputViewModel> inputs;
		Subject<Unit> viewPositionChanged = new Subject<Unit>();

		public NodeViewModel(Node model, PipelineViewModel parent)
		{
			fake = new InOutputViewModel(model: null, parent: this);
			Model = model;
			NodeType = new NodeType { Type = model.GetType() };
			Parent = parent;
			ZIndex = 0;

			inputs = Model.Inputs.Select(i => new InOutputViewModel(i, this)).ToList();
			Outputs = Model.Outputs.Select(i => new InOutputViewModel(i, this)).ToList();

			if (Model.Outputs is INotifyCollectionChanged)
				((INotifyCollectionChanged)Model.Outputs).CollectionChanged += delegate
				{
					Outputs = Model.Outputs.Select(i => new InOutputViewModel(i, this)).ToList();
					NotifyOfPropertyChange(() => Outputs);
					NotifyOfPropertyChange(() => HasOutputs);
					Parent.NotifyOfPropertyChange(() => Parent.Edges);
				};
		}

		public NodeType NodeType { get; private set; }
		public Node Model { get; private set; }
		public PipelineViewModel Parent { get; private set; }
		public IObservable<Unit> ViewPositionChanged { get { return viewPositionChanged; } }

		/// <summary>
		/// Z (depth) index of this node for controlling the render order of overlapping nodes.
		/// This property is not backed by the model because it's really not that important to serialize.
		/// </summary>
		public int ZIndex { get; set; }

		public bool HasOutputs { get { return Model.Outputs.Count != 0; } }

		/// <summary>
		/// Inputs of the model plus a fake input if the node allows adding inputs
		/// </summary>
		public IEnumerable<InOutputViewModel> Inputs
		{
			get
			{
				return Model.UserCanAddInputs ? inputs.Concat(new[] { fake }) : inputs;
			}
		}

		public IEnumerable<InOutputViewModel> Outputs { get; private set; }

		public Point Position
		{
			get { return new Point(Model.X, Model.Y); }
			set
			{
				Model.X = value.X;
				Model.Y = value.Y;
				NotifyOfPropertyChange(() => Margin);
				viewPositionChanged.OnNext(Unit.Default);
			}
		}

		public Thickness Margin { get { return new Thickness(Model.X, Model.Y, 0, 0); } }

		public void RemoveNode()
		{
			/* Only allow this if pipeline is not rendering */
			if (!Parent.Parent.ReplayStateViewModel.IsPlaying) {
				Parent.Parent.CloseWindows(this.Model);
				Parent.Nodes.Remove(this);
				Parent.Parent.Model.Graph.RemoveNode(this.Model);
				Parent.CullAllInputs();
				Parent.Parent.SaveSnapshot();
				Parent.NotifyOfPropertyChange(() => Parent.Edges);
			}
		}

		/// <summary>
		/// Renders an output into a user-chosen file.
		/// </summary>
		public IEnumerable<IResult> SaveNodeOutput(Node.Output output)
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv", OpenReadOnly = false };
			yield return file;
			IoC.Get<IWindowManager>().ShowDialog(new SaveNodeOutputViewModel(output, file.Stream(), Parent.Parent.Model));
		}

		/// <summary>
		/// Opens a new output window for a node output.
		/// </summary>
		public void ShowNodeOutput(Node.Output output)
		{
			Parent.Parent.OpenWindow(new VideoOutputViewModel(output));
		}

		public Node.Input AddInput(Node.Output source)
		{
			var input = new Node.Input { Source = source };
			Model.Inputs.Add(input);
			inputs.Add(new InOutputViewModel(input, this));
			NotifyOfPropertyChange(() => Inputs);
			return input;
		}

		/// <summary>
		/// Culls inputs of this view model's node and their counterparts in this view model.
		/// </summary>
		public void CullInputs()
		{
			Model.CullInputs();
			// cull inputs in this view model
			foreach (InOutputViewModel inputVM in inputs.ToArray()) {
				if (!Model.Inputs.Contains((Node.Input)inputVM.Model)) {
					inputs.Remove(inputVM);
				}
			}
			NotifyOfPropertyChange(() => Inputs);
		}

		public void ViewLoaded()
		{
			viewPositionChanged.OnNext(Unit.Default);
		}
	}
}
