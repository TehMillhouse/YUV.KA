﻿using Xunit;
using YuvKA.Pipeline;

namespace YuvKA.Test.Pipeline
{
	public class PipelineGraphTest
	{
		/// <summary>
		/// Creates a simple graph and then runs DFS on it, afterwards testing 
		/// whether the returned list contains the right nodes.
		/// graph:
		///             [node2]
		///            /
		///     [node0]         [node4]
		///            \       /
		///             [node3]
		///            /
		///     [node1]
		/// </summary>
		[Fact]
		public void DfsFindsCorrectNodes()
		{
			// create graph
			AnonymousNode node0 = new AnonymousNode() { Name = "node0" };
			AnonymousNode node1 = new AnonymousNode() { Name = "node1" };
			AnonymousNode node2 = new AnonymousNode(node0) { Name = "node2" };
			AnonymousNode node3 = new AnonymousNode(node0, node1) { Name = "node3" };
			AnonymousNode node4 = new AnonymousNode(node3) { Name = "node4" };

			PipelineGraph graph = new PipelineGraph {
				Nodes = { node0, node1, node2, node3, node4 }
			};

			Assert.Contains<Node>(node0, graph.DepthFirstSearch(node4));
			Assert.Contains<Node>(node1, graph.DepthFirstSearch(node4));
			Assert.DoesNotContain<Node>(node2, graph.DepthFirstSearch(node4));
			Assert.DoesNotContain<Node>(node4, graph.DepthFirstSearch(node3));
		}

		/// <summary>
		/// Creates a valid graph, then tries to add an illegal edge and
		/// checks whether PipelineGraph detects the invalidity of the resulting graph
		/// graph: (the doubly-drawn arrow is the one that's being added)
		///
		///                     ===================
		///                   //                   \\
		///                  ||                     ||
		///                  \/                     ||
		///     [node0]--->[node1]--->[node2]--->[node3]
		/// </summary>
		[Fact]
		public void PipelineGraphCanDetectCycles()
		{
			// create graph
			AnonymousNode node0 = new AnonymousNode( ) { Name = "node0"};
			AnonymousNode node1 = new AnonymousNode(node0) { Name = "node1" };
			AnonymousNode node2 = new AnonymousNode(node1) { Name = "node2" };
			AnonymousNode node3 = new AnonymousNode(node2) { Name = "node3" };

			PipelineGraph graph = new PipelineGraph { Nodes = { node0, node1, node2, node3} };

			Assert.Equal(false, graph.AddEdge(node3.Outputs[0], node1.Inputs[0]));
		}

		/// <summary>
		/// Creates a small graph with a split pipeline and checks the Pipelinegraph's
		/// bahavior. graph:
		///         [node1]
		///        /       \
		/// [node0]         [node3]---[node4]
		///        \       /
		///         [node2]
		/// </summary>
		[Fact]
		public void PipelineGraphCanHandleSplits()
		{
			// create graph
			AnonymousNode node0 = new AnonymousNode() { Name = "node0" };
			AnonymousNode node1 = new AnonymousNode(node0) { Name = "node1" };
			AnonymousNode node2 = new AnonymousNode(node0) { Name = "node2" };
			AnonymousNode node3 = new AnonymousNode(node1, node2) { Name = "node3" };
			AnonymousNode node4 = new AnonymousNode(node3) { Name = "node4" };

			PipelineGraph graph = new PipelineGraph {
				Nodes = { node0, node1, node2, node3, node4 }
			};

			Assert.Contains<Node>(node0, graph.DepthFirstSearch(node3));
			Assert.Contains<Node>(node1, graph.DepthFirstSearch(node4));
			Assert.Contains<Node>(node2, graph.DepthFirstSearch(node4));
			Assert.DoesNotContain<Node>(node3, graph.DepthFirstSearch(node1));
			Assert.DoesNotContain<Node>(node1, graph.DepthFirstSearch(node2));
		}
	}
}
