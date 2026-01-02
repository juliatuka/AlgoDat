using SudokuSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver_Test
{
	[TestFixture]
	public class GraphTest
	{
		private int[][] EmptyGrid()
		{
			return Enumerable.Range(0, 9)
				.Select(_ => Enumerable.Repeat(0, 9).ToArray())
				.ToArray();
		}

		[Test]
		public void CreatingNewGraph_HasNoNodes_Or_Edges()
		{
			var graph = new Graph<int>();

			Assert.That(graph.NodeCount, Is.EqualTo(0));
			Assert.That(graph.EdgeCount, Is.EqualTo(0));
			Assert.IsFalse(graph.Nodes.Any());
		}

		[Test]
		public void AddingEdges_AddsNodes_And_Edges()
		{
			var graph = new Graph<int>();

			graph.AddEdge(1, 2);

			Assert.That(graph.NodeCount, Is.EqualTo(2));
			Assert.That(graph.EdgeCount, Is.EqualTo(1));

			var neighboursOfA = graph.GetNeighbours(1).ToList();
			Assert.That(neighboursOfA.Count, Is.EqualTo(1));
			Assert.That(neighboursOfA[0], Is.EqualTo(2));
		}

		[Test]
		public void AddingEdges_No_SelfLoops_Are_Created()
		{
			var graph = new Graph<int>();

			graph.AddEdge(1, 1);

			Assert.That(graph.NodeCount, Is.EqualTo(1));
			Assert.That(graph.EdgeCount, Is.EqualTo(0));
			Assert.IsFalse(graph.GetNeighbours(1).Any());
		}

		[Test]
		public void GetNeighbours_OnNonExistingNode_Returns_Empty()
		{
			var graph = new Graph<int>();

			graph.AddEdge(1, 2);

			var neighbours = graph.GetNeighbours(5).ToList();

			Assert.IsEmpty(neighbours);
		}

		[Test]
		public void GetNeighbours_Node_Without_Outgoing_Edges_ReturnsEmpty()
		{
			var graph = new Graph<int>();

			graph.AddEdge(1, 2);

			var neighboursOfTwo = graph.GetNeighbours(2).ToList();

			Assert.IsEmpty(neighboursOfTwo);
		}

		[Test]
		public void GetAllEdges_ReturnsAllEdgesWithCorrectWeights()
		{
			var graph = new Graph<int>();

			graph.AddEdge(1, 2, 2);
			graph.AddEdge(1, 3, 3);
			graph.AddEdge(2, 3, 4);

			var edges = graph.GetAllEdges().ToList();

			Assert.That(edges.Count, Is.EqualTo(3));

			Assert.That(edges, Has.Exactly(1).Matches<(int from, int to, int weight)>(e => e.from == 1 && e.to == 2 && e.weight == 2));
			Assert.That(edges, Has.Exactly(1).Matches<(int from, int to, int weight)>(e => e.from == 1 && e.to == 3 && e.weight == 3));
			Assert.That(edges, Has.Exactly(1).Matches<(int from, int to, int weight)>(e => e.from == 2 && e.to == 3 && e.weight == 4));
		}

		[Test]
		public void CreateGraphFromSudokuGrid_Has81NodesAsAdjacencyKeys()
		{
			var g = new Graph<Node>();
			g.CreateGraphFromSudokuGrid(EmptyGrid());

			Assert.That(g.AdjacencyList.Keys.Count, Is.EqualTo(81));
			Assert.That(g.AdjacencyList.Keys.Min(), Is.EqualTo(0));
			Assert.That(g.AdjacencyList.Keys.Max(), Is.EqualTo(80));
		}

		[Test]
		public void CreateGraphFromSudokuGrid_EachNodeHas20Neighbors()
		{
			var g = new Graph<Node>();
			g.CreateGraphFromSudokuGrid(EmptyGrid());

			foreach (var kvp in g.AdjacencyList)
			{
				Assert.That(kvp.Value.Count, Is.EqualTo(20));
			}
		}

		[Test]
		public void CreateGraphFromSudokuGrid_TotalDirectedEdgesIs1620()
		{
			var g = new Graph<Node>();
			g.CreateGraphFromSudokuGrid(EmptyGrid());

			int directedEdges = g.AdjacencyList.Values.Sum(list => list.Count);
			Assert.That(directedEdges, Is.EqualTo(1620));
		}

		[Test]
		public void CreateGraphFromSudokuGrid_EdgesAreSymmetric()
		{
			var g = new Graph<Node>();
			g.CreateGraphFromSudokuGrid(EmptyGrid());

			foreach (var u in g.AdjacencyList.Keys)
			{
				foreach (var e in g.AdjacencyList[u])
				{
					int v = e.Target;

					Assert.True(
						g.AdjacencyList.ContainsKey(v) &&
						g.AdjacencyList[v].Any(back => back.Target == u)
					);
				}
			}
		}

		[Test]
		public void CreateGraphFromSudokuGrid_NoSelfLoops()
		{
			var g = new Graph<Node>();
			g.CreateGraphFromSudokuGrid(EmptyGrid());

			foreach (var u in g.AdjacencyList.Keys)
			{
				Assert.IsFalse(g.AdjacencyList[u].Any(e => e.Target == u));
			}
		}

		[Test]
		public void CreateGraphFromSudokuGrid_TopLeftCellHasCorrectNeighborhoodShape()
		{
			var g = new Graph<Node>();
			g.CreateGraphFromSudokuGrid(EmptyGrid());

			int u = 0;
			var neigh = g.AdjacencyList[u].Select(e => e.Target).ToHashSet();

			for (int c = 1; c <= 8; c++)
				Assert.IsTrue(neigh.Contains(c));

			for (int r = 1; r <= 8; r++)
				Assert.IsTrue(neigh.Contains(r*9));

			int[] boxIds = { 1, 2, 9, 10, 11, 18, 19, 20 };
			foreach (var id in boxIds)
				Assert.IsTrue(neigh.Contains(id));

			Assert.That(neigh.Count, Is.EqualTo(20));
		}
	}
}
