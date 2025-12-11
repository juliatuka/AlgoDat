using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IslandsInTheGrid
{
	internal struct Edge
	{
		public int Target { get; set; } = -1; // index of the target node, default -1 means not set
		public int Weight { get; set; } = 0; // weight of the edge

		[JsonIgnore]
		public bool IsValid { get; private set; } = false;

		public Edge(int target, int weight)
		{
			Target = target;
			Weight = weight;
			IsValid = true;
		}
	}

	/// <summary>
	/// This is the Graph class. Nodes are of type T. The key for the node is provided with the ToString() method.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Graph<T>
	{
		/// <summary>
		/// each node has unique index
		/// inside the graph th eindex is used.
		/// </summary>
		List<T> nodes = new();

		public IEnumerable<T> Nodes => nodes;

		/// <summary>
		/// get the number of nodes in the graph
		/// </summary>
		public int NodeCount => nodes.Count;

		/// <summary>
		/// get the number of edges in the graph
		/// </summary>  
		public int EdgeCount
		{
			get
			{
				int count = 0;
				foreach (var edges in adjacencyList.Values)
				{
					count += edges.Count;
				}
				return count;
			}
		}

		/// <summary>
		/// get all edges in the graph as tuples (from, to, weight)
		/// </summary>
		public IEnumerable<(T from, T to, int weight)> GetAllEdges()
		{
			foreach (var fromIndex in adjacencyList.Keys)
			{
				foreach (var edge in adjacencyList[fromIndex])
				{
					yield return (nodes[fromIndex], nodes[edge.Target], edge.Weight);
				}
			}
		}

		Dictionary<int, List<Edge>> adjacencyList = new();

		public Graph()
		{
		}

		/// <summary>
		/// get the neighbours for a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public IEnumerable<T> GetNeighbours(T node)
		{
			if (!nodes.Contains(node))
			{
				yield break;
			}

			int index = nodes.IndexOf(node);
			if (adjacencyList.TryGetValue(index, out var neighbors))
			{
				foreach (var neighbor in neighbors)
				{
					yield return nodes[neighbor.Target];
				}
			}
		}

		/// <summary>
		/// add a new edge to the graph. the edge is directed from 'from' to 'to'.
		/// the edge can have an optional weight.
		/// from and to nodes are added to the graph if they do not exist.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="weight"></param>
		public void AddEdge(T from, T to, int weight = 1)
		{
			if (!nodes.Contains(from))
			{
				nodes.Add(from);
			}
			if (!nodes.Contains(to))
			{
				nodes.Add(to);
			}

			int fromIndex = nodes.IndexOf(from);
			int toIndex = nodes.IndexOf(to);

			if (fromIndex == toIndex)
				return; // no self-loops allowed

			if (!adjacencyList.ContainsKey(fromIndex))
			{
				adjacencyList[fromIndex] = new List<Edge>();
			}

			// check if the edge already exists
			// if so, only update the weight
			var existingEdge = adjacencyList[fromIndex].Find(e => e.Target == toIndex);
			if (existingEdge.IsValid && existingEdge.Target == toIndex)
			{
				existingEdge.Weight = weight;
			}
			else
			{
				// if the edge does not exist, add it
				adjacencyList[fromIndex].Add(new Edge(toIndex, weight));
			}
		}

		public bool CreateGraphFromGrid(int[][] grid)
		{
			nodes.Clear();
			adjacencyList.Clear();

			int rows = grid.Length;
			int cols = grid[0].Length;

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					bool isLand = grid[r][c] != 0;

					var node = (T)(object)new Node(r, c, isLand);
					nodes.Add(node);
				}
			}

			(int dr, int dc)[] dirs =
			{
				(1, 0),		// down
				(-1, 0),	// up
				(0, 1),		// right
				(0, -1)		// left
			};

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					if (grid[r][c] == 0)
					{
						continue;
					}

					int fromIndex = r * cols + c; // create 1-dimensional Index
					var fromNode = nodes[fromIndex];

					foreach (var (dr, dc) in dirs)
					{
						int nextr = r + dr;
						int nextc = c + dc;

						if (nextr < 0 || nextr >= rows || nextc < 0 || nextc >= cols)
							continue;

						if (grid[nextr][nextc] == 0)
							continue;

						int toIndex = nextr * cols + nextc; // create 1-dimensional Index
						var toNode = nodes[toIndex];

						AddEdge(fromNode, toNode);
					}
				}
			}

			return true;
		}
	}
}
