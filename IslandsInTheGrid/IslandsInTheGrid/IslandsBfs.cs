using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandsInTheGrid
{
	public class IslandsBfs : BreadthFirstSearch<Node>
	{
		public int CurrentIslandSize { get; set; }
		public int CurrentIslandFlag { get; set; }

		private readonly HashSet<Node> _globalVisited;

		public List<(int size, List<Node> nodes)> _islands; // (size, nodes)

		public IslandsBfs()
		{
			CurrentIslandSize = 0;
			CurrentIslandFlag = 1;
			_islands = new List<(int, List<Node>)>();
			_globalVisited = new HashSet<Node>();
		}

		public override void ProcessNode(Node node)
		{
			base.ProcessNode(node);
			_globalVisited.Add(node);
			CurrentIslandSize++;
			node.IslandFlag = CurrentIslandFlag;
		}

		public List<(int size, List<Node> nodes)> CountIslands(Graph<Node> graph)
		{
			_islands.Clear();
			_globalVisited.Clear();

			foreach (var node in graph.Nodes)
			{
				if (_globalVisited.Contains(node) || node.IslandFlag != 0 || !node.IsLand) // already in some island or water
					continue;

				var island = Search(graph, node);
				_islands.Add((CurrentIslandSize, island));

				// reset size and new flag
				CurrentIslandFlag++;
				CurrentIslandSize = 0;
			}

			return _islands;
		}

		public (int size, List<Node> nodes) SmallestIsland => _islands.Count == 0 ? (0, new List<Node>()) : _islands.MinBy(i => i.size); // returning size 0 and empty list if no islands

		public (int size, List<Node> nodes) LargestIsland => _islands.Count == 0 ? (0, new List<Node>()) : _islands.MaxBy(i => i.size); // returning size 0 and empty list if no islands

	}
}
