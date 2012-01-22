using System;
using System.Collections.Generic;
using System.Linq;
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
		/// Returns the number of frames to precompute, so the specified nodes can process the next frame. The method assumes, that the graph does not contain any cycles.
		/// </summary>
		/// <param name="outputNodes">The specified nodes.</param>
		/// <returns>The number of frames to precompute.</returns>
		public int NumberOfFramesToPrecompute(IEnumerable<Node> outputNodes)
		{
			int precomputeCount = 0;
			foreach (Node node in outputNodes) {
				precomputeCount = Math.Max(precomputeCount, NumberOfFramesToPrecompute(node));
			}
			return precomputeCount;
		}

		/// <summary>
		/// Returns true iff an edge between the specified nodes does not lead to a cycle and can be added.
		/// </summary>
		public bool CanAddEdge(Node source, Node sink)
		{
			return !DepthFirstSearch(source).Contains(sink);
		}

		/// <summary>
		/// Returns true if the specified edge does not lead to a cycle and can be added. Else the method returns false.
		/// </summary>
		/// <param name="source"> The start of the new edge. </param>
		/// <param name="sink"> The end of the new edge. </param>
		/// <returns> True if the new edge has been added, else it returns false.</returns>
		public bool AddEdge(Node.Output source, Node.Input sink)
		{
			IEnumerable<Node> nodeList = DepthFirstSearch(source.Node);
			if (nodeList.Any(node => node.Inputs.Contains(sink))) {
				return false;
			}
			else {
				sink.Source = source;
				return true;
			}
		}

		/// <summary>
		/// Adds the index to the new node's name. The new name will be the first available one of the row NodeName, NodeName 2, NodeName 3, NodeName 4...
		/// </summary>
		/// <param name="node">The new node.</param>
		public void AddNodeWithIndex(Node node)
		{
			bool nameIsFree = false;
			for (int i = 1; i <= (Nodes.Count + 1) && !nameIsFree; i++) {
				nameIsFree = true;
				foreach (Node existingNode in Nodes) {
					if (i == 1 && node.Name == existingNode.Name) {
						nameIsFree = false;
					}
					else if (i > 1 && (node.Name + " " + i) == existingNode.Name) {
						nameIsFree = false;
					}
				}
				if (nameIsFree && i > 1) {
					node.Name += " " + i;
				}
			}
			Nodes.Add(node);
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

			nodeList.AddFirst(startNode);
			visited.Add(startNode);

			Visit(startNode, nodeList, visited);

			return nodeList;
		}

		private int NumberOfFramesToPrecompute(Node startNode)
		{
			int framesToPrecompute = 0;
			if (startNode.Inputs != null) {
				foreach (Node.Input input in startNode.Inputs) {
					if (input.Source != null) {
						framesToPrecompute = Math.Max(framesToPrecompute, NumberOfFramesToPrecompute(input.Source.Node));
					}
				}
			}
			framesToPrecompute += startNode.NumberOfFramesToPrecompute;
			return framesToPrecompute;
		}

		/**
		 * Recursive part of the depth first search.
		 */
		private void Visit(Node node, LinkedList<Node> nodeList, HashSet<Node> visited)
		{
			if (node.Inputs != null) {
				foreach (Node.Input input in node.Inputs) {
					if (input.Source != null) {
						Node child = input.Source.Node;
						if (nodeList.Contains(child) && visited.Contains(child)) {
							nodeList.AddLast(child);
							//Cancel further search here, since a cycle has been found
							return;
						}
						else if (!nodeList.Contains(child)) {
							nodeList.AddLast(child);
							visited.Add(child);
							if (child.Inputs != null) {
								Visit(child, nodeList, visited);
							}
						}
					}
				}
			}
			visited.Remove(node);
		}
	}
}
