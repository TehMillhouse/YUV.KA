using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class NodeViewModel : PropertyChangedBase
	{
		InOutputViewModel fake = new InOutputViewModel(model: null);
		IList<InOutputViewModel> inputs;

		public NodeViewModel(Node model, PipelineViewModel parent)
		{
			Model = model;
			NodeType = new NodeType { Type = model.GetType() };
			Parent = parent;

			inputs = Model.Inputs.Select(i => new InOutputViewModel(i)).ToList();
			Outputs = Model.Outputs.Select(i => new InOutputViewModel(i)).ToList();

			if (Model.Outputs is INotifyCollectionChanged)
				((INotifyCollectionChanged)Model.Outputs).CollectionChanged += delegate
				{
					Outputs = Model.Outputs.Select(i => new InOutputViewModel(i)).ToList();
					NotifyOfPropertyChange(() => Outputs);
					Parent.NotifyOfPropertyChange(() => Parent.Edges);
				};
		}

		public NodeType NodeType { get; private set; }
		public Node Model { get; private set; }
		public PipelineViewModel Parent { get; private set; }

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
				Parent.NotifyOfPropertyChange(() => Parent.Edges);
			}
		}

		public Thickness Margin { get { return new Thickness(Model.X, Model.Y, 0, 0); } }

		public void RemoveNode()
		{
			Parent.Nodes.Remove(this);
			Parent.Parent.Model.Graph.Nodes.Remove(this.Model);
			Parent.NotifyOfPropertyChange(() => Parent.Edges);
		}

		public IEnumerable<IResult> SaveNodeOutput(Node.Output output)
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv", OpenReadOnly = false };
			yield return file;
			IoC.Get<IWindowManager>().ShowDialog(new SaveNodeOutputViewModel(output, file.Stream, Parent.Parent.Model));
		}

		public void ShowNodeOutput(Node.Output output)
		{
			Parent.Parent.OpenWindow(new VideoOutputViewModel(output));
		}

	}
}
