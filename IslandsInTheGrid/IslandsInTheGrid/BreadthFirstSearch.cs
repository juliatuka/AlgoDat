using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandsInTheGrid
{
	public class BreadthFirstSearch<T>
	{
		Boolean[]? visited;
		List<T>? result;

		// use a queue
		Queue<T> queue = new Queue<T>();

		public List<T> Search(Graph<T> graph, T start)
		{
			visited = new Boolean[graph.NodeCount];
			result = new List<T>();

			queue.Enqueue(start);

			BFS(graph);

			return result;
		}

		/// <summary>
		/// use this function to process the node.
		/// </summary>
		/// <param name="node"></param>
		public virtual void ProcessNode(T node)
		{
			// default implementation does nothing
			// override this method to process the node
		}

		void BFS(Graph<T> graph)
		{
			while (queue.Count > 0)
			{
				T node = queue.Dequeue();
				int nodeIndex = graph.Nodes.ToList().IndexOf(node);
				if (visited![nodeIndex])
				{
					continue; // skip already visited nodes
				}

				ProcessNode(node); // process the current node
				visited[nodeIndex] = true;

				result!.Add(graph.Nodes.ElementAt(nodeIndex));

				foreach (var neighbor in graph.GetNeighbours(graph.Nodes.ElementAt(nodeIndex)))
				{
					int neighborIndex = graph.Nodes.ToList().IndexOf(neighbor);
					if (!visited[neighborIndex])
					{
						queue.Enqueue(neighbor);
					}
				}
			}
		}

	}
}
