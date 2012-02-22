using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace YuvKA.Pipeline
{
	/// <summary>
	/// This class manages the basic functionalities of the pipeline.
	/// </summary>
	[DataContract]
	public class PipelineGraph : PropertyChangedBase
	{
		public PipelineGraph()
		{
			Nodes = new List<Node>();
		}

		/// <summary>
		/// The list of nodes contained in the pipeline.
		/// </summary>
		[DataMember]
		public IList<Node> Nodes { get; private set; }

		/// <summary>
		/// Represents the number of frames of the longest finit video input in the pipeline. If all inputs have infinite length the value is null.
		/// </summary>
		public int? TickCount
		{
			get
			{
				return Nodes.OfType<InputNode>().Max(node => node.TickCount);
			}
		}

		/// <summary>
		/// Returns the number of ticks which are necessary to let the specified nodes process the next tick. The method assumes, that the graph does not contain any cycles.
		/// </summary>
		public int NumberOfTicksToPrecompute(IEnumerable<Node> outputNodes)
		{
			int precomputeCount = 0;
			foreach (Node node in outputNodes) {
				precomputeCount = Math.Max(precomputeCount, NumberOfTicksToPrecompute(node));
			}
			return precomputeCount;
		}

		/// <summary>
		/// Returns true iff an edge between the specified nodes does not lead to a cycle.
		/// </summary>
		public bool CanAddEdge(Node source, Node sink)
		{
			return !DepthFirstSearch(source).Contains(sink);
		}

		/// <summary>
		/// Returns true and adds the edge iff the specified directed edge does not lead to a cycle.
		/// </summary>
		public bool AddEdge(Node.Output source, Node.Input sink)
		{
			IEnumerable<Node> nodeList = DepthFirstSearch(source.Node);
			// if sink can be reached after starting dfs at the source the new edge would create a cycle
			if (nodeList.Any(node => node.Inputs.Contains(sink))) {
				return false;
			}
			else {
				sink.Source = source;
				return true;
			}
		}

		/// <summary>
		/// Adds an index to the specified node's name. The new name will be the first available one of the row NodeName, NodeName 2, NodeName 3, NodeName 4...
		/// </summary>
		public void AddNode(Node node)
		{
			bool nameIsFree = false;
			// walk through nodes and look for first free index
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
				// special case: "NodeName 1" shall be known as "NodeName"
				if (nameIsFree && i > 1) {
					node.Name += " " + i;
				}
			}
			if (node is InputNode) {
				NotifyOfPropertyChange(() => TickCount);
				node.PropertyChanged += (sender, e) => {
					if (e.PropertyName == "TickCount")
						NotifyOfPropertyChange(() => TickCount);
				};
			}
			Nodes.Add(node);
		}

		/// <summary>
		/// Removes the specified node from the node list and removes all edges connected with the node.
		/// </summary>
		public void RemoveNode(Node node)
		{
			this.Nodes.Remove(node);
			foreach (Node checkedNode in Nodes) {
				foreach (Node.Input potentialDependency in checkedNode.Inputs) {
					if (potentialDependency.Source != null && potentialDependency.Source.Node == node) {
						// delete edge to node
						potentialDependency.Source = null;
					}
				}
			}
			NotifyOfPropertyChange(() => TickCount);
		}

		/// <summary>
		/// Returns a list which contains all nodes which can be reached from the start node.
		/// </summary>
		public IEnumerable<Node> DepthFirstSearch(Node startNode)
		{
			LinkedList<Node> nodeList = new LinkedList<Node>();
			HashSet<Node> visited = new HashSet<Node>();

			nodeList.AddFirst(startNode);
			visited.Add(startNode);

			Visit(startNode, nodeList, visited);

			return nodeList;
		}

		// Recursive part of NumberOfFramesToPrecompute.
		private int NumberOfTicksToPrecompute(Node startNode)
		{
			int framesToPrecompute = 0;
			if (startNode.Inputs != null) {
				foreach (Node.Input input in startNode.Inputs) {
					if (input.Source != null) {
						framesToPrecompute = Math.Max(framesToPrecompute, NumberOfTicksToPrecompute(input.Source.Node));
					}
				}
			}
			framesToPrecompute += startNode.NumberOfFramesToPrecompute;
			return framesToPrecompute;
		}

		// Recursive part of the depth first search.
		private void Visit(Node node, LinkedList<Node> nodeList, HashSet<Node> visited)
		{
			if (node.Inputs != null) {
				foreach (Node.Input input in node.Inputs) {
					if (input.Source != null) {
						Node child = input.Source.Node;
						if (nodeList.Contains(child) && visited.Contains(child)) {
							// cancel further search here, since a cycle has been found
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
