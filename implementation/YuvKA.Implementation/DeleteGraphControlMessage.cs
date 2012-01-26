namespace YuvKA.Implementation
{
	public class DeleteGraphControlMessage
	{
		private GraphControl toDelete;

		public DeleteGraphControlMessage(GraphControl toDelete)
		{
			this.toDelete = toDelete;
		}

		public GraphControl GraphControltoDelete
		{
			get { return toDelete; }
		}
	}

	public class GraphTypeChosenMessage
	{
		private GraphControl graphControl;

		public GraphTypeChosenMessage(GraphControl toDelete)
		{
			this.graphControl = toDelete;
		}

		public GraphControl GraphControl
		{
			get { return graphControl; }
		}
	}
}
