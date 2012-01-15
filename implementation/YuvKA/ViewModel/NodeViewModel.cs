using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	public class NodeViewModel
	{
		public NodeViewModel(Node nodeModel, MainViewModel parent)
		{
			NodeModel = nodeModel;
			NodeType = new NodeType { Type = nodeModel.GetType() };
			Parent = parent;
		}

		public NodeType NodeType { get; private set; }
		public Node NodeModel { get; private set; }
		public MainViewModel Parent { get; private set; }

		public Thickness Margin { get { return new Thickness(NodeModel.X, NodeModel.Y, 0, 0); } }

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
	}
}
