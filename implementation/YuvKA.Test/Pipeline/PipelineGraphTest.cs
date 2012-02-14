using System.Collections.Generic;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

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
			AnonymousNode node0 = new AnonymousNode() { Name = "node0" };
			AnonymousNode node1 = new AnonymousNode(node0) { Name = "node1" };
			AnonymousNode node2 = new AnonymousNode(node1) { Name = "node2" };
			AnonymousNode node3 = new AnonymousNode(node2) { Name = "node3" };
			PipelineGraph graph = new PipelineGraph { Nodes = { node0, node1, node2, node3 } };

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





		/// <summary>
		/// Creates a small graph and checks ReturnNumberOfFramesToPrecompute's behavior.
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
		public void TestReturnNumberOfFramesToPrecompute()
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
			//set number of frames to precompute in the graph's nodes
			node0.SettableNumberOfFramesToPrecompute = 0;
			node1.SettableNumberOfFramesToPrecompute = 1;
			node2.SettableNumberOfFramesToPrecompute = 2;
			node3.SettableNumberOfFramesToPrecompute = 4;
			node4.SettableNumberOfFramesToPrecompute = 8;

			List<AnonymousNode> nodeList = new List<AnonymousNode>();
			nodeList.Add(node0);
			Assert.Equal(0, graph.NumberOfFramesToPrecompute(nodeList));
			nodeList.Add(node1);
			Assert.Equal(1, graph.NumberOfFramesToPrecompute(nodeList));
			nodeList.Add(node2);
			Assert.Equal(2, graph.NumberOfFramesToPrecompute(nodeList));
			nodeList.Add(node3);
			Assert.Equal(5, graph.NumberOfFramesToPrecompute(nodeList));
			nodeList.Add(node4);
			Assert.Equal(13, graph.NumberOfFramesToPrecompute(nodeList));
		}

		/// <summary>
		/// Creates a simple graph and then removes a node.
		/// graph:
		///     [node0]         [node3]
		///            \       /
		///             [node2]
		///            /	   \
		///     [node1]			[node4]
		/// </summary>
		[Fact]
		public void TestRemoveNode()
		{
			// create graph
			AnonymousNode node0 = new AnonymousNode() { Name = "node0" };
			AnonymousNode node1 = new AnonymousNode() { Name = "node1" };
			AnonymousNode node2 = new AnonymousNode(node0, node1) { Name = "node2" };
			AnonymousNode node3 = new AnonymousNode(node2) { Name = "node3" };
			AnonymousNode node4 = new AnonymousNode(node2) { Name = "node4" };
			PipelineGraph graph = new PipelineGraph {
				Nodes = { node0, node1, node2, node3, node4 }
			};

			graph.RemoveNode(node2);

			foreach(Node.Input input in node3.Inputs) 
				Assert.Equal(null, input.Source);

			foreach (Node.Input input in node4.Inputs)
				Assert.Equal(null, input.Source);
		}


		/// <summary>
		/// Creates some nodes and checks the behaviour of the AddNodeWithIndex method.
		/// </summary>
		[Fact]
		public void TestAddNodeWithIndex()
		{
			// create some nodes and initialize graph
			BlurNode blur0 = new BlurNode();
			BlurNode blur1 = new BlurNode();
			BlurNode blur2 = new BlurNode();
			BlurNode blur3 = new BlurNode();
			BlurNode blur4 = new BlurNode();
			DelayNode delay0 = new DelayNode();
			DelayNode delay1 = new DelayNode();
			PipelineGraph graph = new PipelineGraph();

			graph.AddNodeWithIndex(blur0);
			graph.AddNodeWithIndex(blur1);
			graph.AddNodeWithIndex(delay0);
			graph.AddNodeWithIndex(blur2);
			graph.AddNodeWithIndex(delay1);

			Assert.Equal("Blur", blur0.Name);
			Assert.Equal("Blur 2", blur1.Name);
			Assert.Equal("Blur 3", blur2.Name);
			Assert.Equal("Delay", delay0.Name);
			Assert.Equal("Delay 2", delay1.Name);

			graph.RemoveNode(blur1);
			graph.AddNodeWithIndex(blur3);
			graph.AddNodeWithIndex(blur4);

			Assert.Equal("Blur 2", blur3.Name);
			Assert.Equal("Blur 4", blur4.Name);
		}
	}
}
