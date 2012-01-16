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
		public NodeViewModel(Node model, MainViewModel parent)
		{
			Model = model;
			NodeType = new NodeType { Type = model.GetType() };
			Parent = parent;

			if (Model.Outputs is INotifyCollectionChanged)
				((INotifyCollectionChanged)Model.Outputs).CollectionChanged +=
					(sender, e) => NotifyOfPropertyChange(() => Outputs);
		}

		public NodeType NodeType { get; private set; }
		public Node Model { get; private set; }
		public MainViewModel Parent { get; private set; }

		public IEnumerable<InOutputViewModel> Inputs
		{
			get
			{
				var inputs = Model.Inputs.Select(i => new InOutputViewModel(i));
				if (Model.UserCanAddInputs)
					inputs = inputs.Concat(new[] { new InOutputViewModel(null) });
				return inputs;
			}
		}

		public IEnumerable<InOutputViewModel> Outputs { get { return Model.Outputs.Select(i => new InOutputViewModel(i)); } }

		public Point Position
		{
			get { return new Point(Model.X, Model.Y); }
			set
			{
				Model.X = value.X;
				Model.Y = value.Y;
				NotifyOfPropertyChange(() => Margin);
			}
		}

		public Thickness Margin { get { return new Thickness(Model.X, Model.Y, 0, 0); } }

		public void RemoveNode()
		{
			Parent.PipelineViewModel.Nodes.Remove(this);
			Parent.Model.Graph.Nodes.Remove(this.NodeModel);
		}

		public IEnumerable<IResult> SaveNodeOutput(Node.Output output)
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv", OpenReadOnly = false };
			yield return file;
			IoC.Get<IWindowManager>().ShowDialog(new SaveNodeOutputViewModel(output, file.Stream, Parent.Model));
		}

		public void ShowNodeOutput(Node.Output output)
		{
			Parent.OpenWindow(new VideoOutputViewModel(output));
		}

		public class InOutputViewModel
		{
			public InOutputViewModel(object model)
			{
				Model = model;
			}

			public object Model { get; private set; }
			public bool IsFake { get { return Model == null; } }
		}
	}
}
