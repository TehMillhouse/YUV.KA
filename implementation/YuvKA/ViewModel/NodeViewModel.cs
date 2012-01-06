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

		public void SaveNodeOutput(Node.Output output)
		{
			throw new System.NotImplementedException();
		}

		public void ShowNodeOutput(Node.Output output)
		{
			Parent.OpenWindow(new VideoOutputViewModel(output));
		}
	}
}
