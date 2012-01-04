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
		///         [firstblur]
		///        /
		/// [input]              [bcs]
		///        \            /
		///         [secondblur]
		/// </summary>
		[Fact]
		public void DfsFindsCorrectNodes()
		{
			// create nodes
			VideoInputNode input = new VideoInputNode();
			BlurNode firstblur = new BlurNode();
			BlurNode secondblur = new BlurNode();
			BrightnessContrastSaturationNode bcs = new BrightnessContrastSaturationNode();

			// set up edges
			firstblur.Inputs[0].Source = input.Outputs[0];
			secondblur.Inputs[0].Source = input.Outputs[0];
			bcs.Inputs[0].Source = secondblur.Outputs[0];

			PipelineGraph graph = new PipelineGraph {
				Nodes = { firstblur, secondblur, bcs, input }
			};

			Assert.Contains<Node>(input, graph.DepthFirstSearch(bcs));
			Assert.DoesNotContain<Node>(firstblur, graph.DepthFirstSearch(bcs));
			Assert.DoesNotContain<Node>(bcs, graph.DepthFirstSearch(secondblur));
		}

		/// <summary>
		/// Creates a valid graph, then tries to add an illegal edge and
		/// checks whether PipelineGraph detects the invalidity of the resulting graph
		/// graph: (the doubly-drawn arrow is the one that's being added)
		///     ===========================
		///   //                            \\
		///    ==>[firstblur]--->[secondblur]------>[bcs]
		/// </summary>
		[Fact]
		public void PipelineGraphCanDetectCycles()
		{
			// create nodes
			BlurNode firstblur = new BlurNode();
			BlurNode secondblur = new BlurNode();
			BrightnessContrastSaturationNode bcs = new BrightnessContrastSaturationNode();

			// set up edges
			secondblur.Inputs[0].Source = firstblur.Outputs[0];
			bcs.Inputs[0].Source = secondblur.Outputs[0];

			PipelineGraph graph = new PipelineGraph { Nodes = { firstblur, bcs, secondblur } };

			Assert.Equal(false, graph.AddEdge(secondblur.Outputs[0], firstblur.Inputs[0]));
		}

		/// <summary>
		/// Creates a small graph with a split pipeline and checks the Pipelinegraph's
		/// bahavior. graph:
		///         [firstblur]
		///        /            \
		/// [input]              [mergeNode]----[bcs]
		///        \            /
		///         [secondblur]
		/// </summary>
		[Fact]
		public void PipelineGraphCanHandleSplits()
		{
			// create nodes
			VideoInputNode input = new VideoInputNode();
			BlurNode firstblur = new BlurNode();
			BlurNode secondblur = new BlurNode();
			BrightnessContrastSaturationNode bcs = new BrightnessContrastSaturationNode();
			AnonymousNode mergeNode = new AnonymousNode((herp, derp) => { return null; }, new Node.Output[] { firstblur.Outputs[0], bcs.Outputs[0] });

			// set up edges
			firstblur.Inputs[0].Source = input.Outputs[0];
			secondblur.Inputs[0].Source = input.Outputs[0];
			bcs.Inputs[0].Source = mergeNode.Outputs[0];

			PipelineGraph graph = new PipelineGraph {
				Nodes = { firstblur, secondblur, bcs, input, mergeNode }
			};

			Assert.Contains<Node>(input, graph.DepthFirstSearch(mergeNode));
			Assert.Contains<Node>(firstblur, graph.DepthFirstSearch(bcs));
			Assert.Contains<Node>(secondblur, graph.DepthFirstSearch(bcs));
			Assert.DoesNotContain<Node>(bcs, graph.DepthFirstSearch(secondblur));
			Assert.DoesNotContain<Node>(firstblur, graph.DepthFirstSearch(secondblur));
		}
	}
}
