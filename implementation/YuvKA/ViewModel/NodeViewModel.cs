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
	public class NodeViewModel : PropertyChangedBase
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
					Parent.NotifyOfPropertyChange(() => Parent.Edges);
				};
		}

		public NodeType NodeType { get; private set; }
		public Node Model { get; private set; }
		public PipelineViewModel Parent { get; private set; }
		public IObservable<Unit> ViewPositionChanged { get { return viewPositionChanged; } }
		public int ZIndex { get; set; }

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
			/* Only allow this if pipline is not rendering */
			if (!Parent.Parent.ReplayStateViewModel.IsPlaying) {
				Parent.Parent.CloseWindow(this.Model);
				Parent.Nodes.Remove(this);
				Parent.Parent.Model.Graph.RemoveNode(this.Model);
				Parent.NotifyOfPropertyChange(() => Parent.Edges);
			}
		}

		public IEnumerable<IResult> SaveNodeOutput(Node.Output output)
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv", OpenReadOnly = false };
			yield return file;
			IoC.Get<IWindowManager>().ShowDialog(new SaveNodeOutputViewModel(output, file.Stream(), Parent.Parent.Model));
		}

		public void ShowNodeOutput(Node.Output output)
		{
			Parent.Parent.OpenWindow(new VideoOutputViewModel(output));
		}

		public Node.Input AddInput()
		{
			var input = new Node.Input();
			Model.Inputs.Add(input);
			inputs.Add(new InOutputViewModel(input, this));
			NotifyOfPropertyChange(() => Inputs);
			return input;
		}

		public void ViewLoaded()
		{
			viewPositionChanged.OnNext(Unit.Default);
		}
	}
}
