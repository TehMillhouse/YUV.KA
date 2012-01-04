using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline
{
	[DataContract]
	public class PipelineGraph
	{
		public PipelineGraph()
		{
			Nodes = new List<Node>();
		}

		[DataMember]
		public IList<Node> Nodes { get; private set; }

		[DataMember]
		public int TickCount
		{
			get
			{
				int tickCount = 0;
				foreach (Node node in Nodes) {
					if (node is InputNode && ((InputNode)node).TickCount > tickCount) {
						tickCount = ((InputNode)node).TickCount;
					}
				}
				return tickCount;
			}
		}


		/// <summary>
		/// Returns true if the specified edge doesnt lead to a cycle. Else the method returns false.
		/// </summary>
		/// <param name="source"> The start of the new edge. </param>
		/// <param name="sink"> The end of the new edge. </param>
		/// <returns> True if the new edge has been added, else </returns>
		public bool AddEdge(Node.Output source, Node.Input sink)
		{
			bool added = true;
			if (source == null || sink == null || ContainsCycle(source.Node)) {
				added = false;
			}
			else {
				sink.Source = source;
			}

			return added;
		}

		/// <summary>
		/// Returns a list which contains those Nodes who are necessary to let the specified Node process.
		/// If the Graph contains a cycle, the returned list will contain one of the cycle-Nodes twice.
		/// Otherwise every Node in the list is unique.
		/// </summary>
		/// <param name="startNode"> The specified Node. </param>
		/// <returns> Returns the list of necessary Nodes.</returns>
		public IEnumerable<Node> DepthFirstSearch(Node startNode)
		{
			LinkedList<Node> nodeList = new LinkedList<Node>();
			HashSet<Node> visited = new HashSet<Node>();

			nodeList.AddLast(startNode);
			visited.Add(startNode);

			foreach (Node.Input input in startNode.Inputs) {
				Visit(input.Source.Node, nodeList, visited);
			}

			return nodeList;
		}

		/**
		 * Recursive part of the depth first search.
		 */
		private void Visit(Node node, LinkedList<Node> nodeList, HashSet<Node> visited)
		{
			if (nodeList.Contains(node) && visited.Contains(node)) {
				nodeList.AddLast(node);
				//Cancel further search here, since a cycle has been found
				return;
			}
			else if (!nodeList.Contains(node)) {
				nodeList.AddLast(node);
				visited.Add(node);

				foreach (Node.Input input in node.Inputs) {
					Visit(input.Source.Node, nodeList, visited);
				}
			}

			visited.Remove(node);

			return;
		}

		//Returns true, if a cycle that includes the specified Node exists.
		private bool ContainsCycle(Node startNode)
		{
			IEnumerable<Node> nodeList = DepthFirstSearch(startNode);
			IEnumerator<Node> enumerator = nodeList.GetEnumerator();
			bool containsCycle = false;

			while (enumerator.MoveNext()) {
				IEnumerator<Node> comparator = nodeList.GetEnumerator();
				while (comparator.MoveNext()) {
					if (enumerator == comparator) {
						containsCycle = true;
					}
				}
			}

			return containsCycle;
		}
	}
}
