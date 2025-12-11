using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IslandsInTheGrid;

namespace IslandsInTheGrid_Test
{
	[TestFixture]
	public class IslandsBfs_Test
	{
		public Graph<Node> BuildGraph(int[][] grid)
		{
			var graph = new Graph<Node>();
			graph.CreateGraphFromGrid(grid);
			return graph;
		}

		[Test]
		public void IslandsBfs_Constructor_InitializesStateCorrectly()
		{
			var islandbfs = new IslandsBfs();

			Assert.That(islandbfs.CurrentIslandSize, Is.EqualTo(0));
			Assert.That(islandbfs.CurrentIslandFlag, Is.EqualTo(1));
			Assert.That(islandbfs._islands, Is.Not.Null);
			Assert.That(islandbfs._islands.Count, Is.EqualTo(0));
		}

		[Test]
		public void IslandsBfs_ProcessNode_IncrementsSize_And_SetsIslandFlag()
		{
			var islandbfs = new IslandsBfs();
			islandbfs.CurrentIslandFlag = 3;
			islandbfs.CurrentIslandSize = 0;

			var node = new Node(0, 0, true);

			islandbfs.ProcessNode(node);

			Assert.That(islandbfs.CurrentIslandSize, Is.EqualTo(1));
			Assert.That(node.IslandFlag, Is.EqualTo(3));
		}

		[Test]
		public void IslandsBfs_CountIslands_OnEmpty_Graph_ReturnsNoIslands()
		{
			var graph = new Graph<Node>();
			var islandbfs = new IslandsBfs();

			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands, Is.Empty);
			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(0));
			Assert.That(islandbfs.SmallestIsland.nodes, Is.Empty);
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(0));
			Assert.That(islandbfs.LargestIsland.nodes, Is.Empty);
		}

		[Test]
		public void IslandsBfs_CountIslands_WithoutLand_ReturnsNoIslands()
		{
			int[][] grid =
			{
				[0, 0, 0,],
				[0, 0, 0,],
				[0, 0, 0,]
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands, Is.Empty);
			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(0));
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(0));


			foreach (var node in graph.Nodes)
			{
				Assert.IsFalse(node.IsLand && node.IslandFlag != 0);
			}
		}

		[Test]
		public void IslandsBfs_CountIslands_AllLand_ReturnsOneIsland()
		{
			int[][] grid =
			{
				[1, 1, 1,],
				[1, 1, 1,],
				[1, 1, 1,]
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands.Count, Is.EqualTo(1));
			var island = islands[0];

			Assert.That(island.size, Is.EqualTo(9));
			Assert.That(island.nodes.Count, Is.EqualTo(9));

			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(9));
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(9));

			var flags = island.nodes.Select(n => n.IslandFlag).Distinct().ToList();
			Assert.That(flags.Count, Is.EqualTo(1));
			Assert.That(flags[0], Is.EqualTo(1));
		}

		[Test]
		public void IslandBfs_CountIslands_Multiple_Islands_ReturnsCorrectNumber_And_Sizes()
		{
			int[][] grid =
			{
				[1, 1, 0, 0],
				[1, 0, 0, 1],
				[0, 0, 1, 1],
				[1, 0, 0, 0]
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands.Count, Is.EqualTo(3));

			var sizes = islands.Select(i => i.size).OrderBy(x => x).ToArray();
			Assert.That(sizes, Is.EqualTo(new[] { 1, 3, 3 }));

			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(1));
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(3));

			Assert.That(islands[0].nodes.Select(n => n.IslandFlag).Distinct().Single(), Is.EqualTo(1));
			Assert.That(islands[1].nodes.Select(n => n.IslandFlag).Distinct().Single(), Is.EqualTo(2));
			Assert.That(islands[2].nodes.Select(n => n.IslandFlag).Distinct().Single(), Is.EqualTo(3));
		}

		[Test]
		public void IslandBfs_CountIslands_MultipleIslands_DiagonalNeighbors_ReturnsSeperateIslands()
		{
			int[][] grid =
			{
				[1, 0],
				[0, 1],
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands.Count, Is.EqualTo(2));

			Assert.IsTrue(islands.All(i=> i.size == 1));

			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(1));
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(1));

			Assert.That(islands[0].nodes.Select(n => n.IslandFlag).Distinct().Single(), Is.EqualTo(1));
			Assert.That(islands[1].nodes.Select(n => n.IslandFlag).Distinct().Single(), Is.EqualTo(2));
		}

		[Test]
		public void IslandBfs_CountIslands_Single_Row_Grid_ReturnsCorrectNumber_And_Sizes()
		{
			int[][] grid =
			{
				[1, 1, 0, 0],
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands.Count, Is.EqualTo(1));

			var island = islands[0];

			Assert.That(island.size, Is.EqualTo(2));
			Assert.That(island.nodes.Count, Is.EqualTo(2));

			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(2));
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(2));

			var flags = island.nodes.Select(n => n.IslandFlag).Distinct().ToList();
			Assert.That(flags.Count, Is.EqualTo(1));
			Assert.That(flags[0], Is.EqualTo(1));
		}

		[Test]
		public void IslandBfs_CountIslands_Single_Column_Grid_ReturnsCorrectNumber_And_Sizes()
		{
			int[][] grid =
			{
				[0],
				[1],
				[1],
				[1],
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands.Count, Is.EqualTo(1));

			var island = islands[0];

			Assert.That(island.size, Is.EqualTo(3));
			Assert.That(island.nodes.Count, Is.EqualTo(3));

			Assert.That(islandbfs.SmallestIsland.size, Is.EqualTo(3));
			Assert.That(islandbfs.LargestIsland.size, Is.EqualTo(3));

			var flags = island.nodes.Select(n => n.IslandFlag).Distinct().ToList();
			Assert.That(flags.Count, Is.EqualTo(1));
			Assert.That(flags[0], Is.EqualTo(1));
		}

		[Test]
		public void IslandBfs_Smallest_And_LargestIsland_ResturnsCorrectIslands()
		{
			int[][] grid =
			{
				[1, 0, 1],
				[1, 0, 1],
				[0, 0, 1],
			};

			var graph = BuildGraph(grid);
			var islandbfs = new IslandsBfs();
			var islands = islandbfs.CountIslands(graph);

			Assert.That(islands.Count, Is.EqualTo(2));

			var smallest = islandbfs.SmallestIsland;
			var largest = islandbfs.LargestIsland;

			Assert.That(smallest.size, Is.EqualTo(2));
			Assert.That(largest.size, Is.EqualTo(3));

			var smallestCoordinates = smallest.nodes.Select(n => (n.Row, n.Column)).OrderBy(r => r.Row).ThenBy(c => c.Column).ToArray();
			var largestCoordinates = largest.nodes.Select(n => (n.Row, n.Column)).OrderBy(r => r.Row).ThenBy(c => c.Column).ToArray();

			Assert.That(smallestCoordinates, Is.EqualTo(new[] {(0, 0), (1, 0)}));
			Assert.That(largestCoordinates, Is.EqualTo(new[] {(0, 2), (1, 2), (2, 2)}));
		}

		[Test]
		public void IslandBfs_Smallest_And_LargestIsland_WithoutIslands_ReturnsZeroAndEmptyList()
		{
			var islandbfs = new IslandsBfs();

			var smallest = islandbfs.SmallestIsland;
			var largest = islandbfs.LargestIsland;

			Assert.That(smallest.size, Is.EqualTo(0));
			Assert.That(smallest.nodes, Is.Empty);
			Assert.That(largest.size, Is.EqualTo(0));
			Assert.That(largest.nodes, Is.Empty);
		}
	}
}
