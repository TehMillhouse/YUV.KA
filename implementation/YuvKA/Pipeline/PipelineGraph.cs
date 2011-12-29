using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline
{
	[DataContract]
	public class PipelineGraph
	{
		[DataMember]
		public IList<Node> Nodes
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[DataMember]
		public int TickCount
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public bool AddEdge(Node.Output source, Node.Input sink)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<Node> DepthFirstSearch(Node startNode)
		{
            LinkedList<Node> NodeList = new LinkedList<Node>();
            HashSet<Node> Visited = new HashSet<Node>();

            NodeList.AddLast(startNode);
            Visited.Add(startNode);

            foreach (Node.Input Input in startNode.Inputs)
            {
                Visit(Input.Source.Node, NodeList, Visited);
            }

            return NodeList;
		}

        private void Visit(Node node, LinkedList<Node> nodeList, HashSet<Node> visited) 
        {

            if (nodeList.Contains(node) && visited.Contains(node))
            {
				nodeList.AddLast(node);
                return;
            } 
			else if (!nodeList.Contains(node)) 
            {
				nodeList.AddLast(node);
				visited.Add(node);

                foreach (Node.Input Input in node.Inputs) 
                {
                    Visit(Input.Source.Node, nodeList, visited);
                }
            }

            visited.Remove(node);

			return;
        }


	}
}
