namespace YuvKA.Implementation
{
	public class DeleteGraphControlMessage
	{
		private GraphControl toDelete;

		public DeleteGraphControlMessage(GraphControl toDelete)
		{
			this.toDelete = toDelete;
		}

		public GraphControl GraphViewtoDelete
		{
			get { return toDelete; }
		}
	}
}
