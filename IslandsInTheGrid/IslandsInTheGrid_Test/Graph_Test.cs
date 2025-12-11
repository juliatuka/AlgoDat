using IslandsInTheGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandsInTheGrid_Test
{
	[TestFixture]
	public class Graph_Test
	{
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
		public void CreatingGraphFromGrid_WithoutIslands_HasNodesButNoEdges()
		{
			var graph = new Graph<Node>();
			int[][] grid =
			{
				[0, 0, 0],
				[0, 0, 0],
				[0, 0, 0]
			};

			bool created = graph.CreateGraphFromGrid(grid);

			Assert.IsTrue(created);
			Assert.That(graph.NodeCount, Is.EqualTo(9)); 
			Assert.That(graph.EdgeCount, Is.EqualTo(0));

			foreach (var nodeObj in graph.Nodes)
			{
				var node = (Node)nodeObj;
				Assert.IsFalse(node.IsLand);
			}
		}

		[Test]
		public void CreatingGraphFromGrid_AllLand_CreatesEdgesBetweenAllNeighbors()
		{
			var graph = new Graph<Node>();
			int[][] grid =
			{
				[1, 1],
				[1, 1]
			};

			graph.CreateGraphFromGrid(grid);
			Assert.That(graph.NodeCount, Is.EqualTo(4));

			var nodes = graph.Nodes.Cast<Node>().ToList();

			var topLeft = nodes.Single(n => n.Row == 0 && n.Column == 0);
			var neighboursTL = graph.GetNeighbours(topLeft).Cast<Node>().ToList();

			Assert.That(neighboursTL.Count, Is.EqualTo(2));
			Assert.IsTrue(neighboursTL.Any(n => n.Row == 0 && n.Column == 1));
			Assert.IsTrue(neighboursTL.Any(n => n.Row == 1 && n.Column == 0));

			var topRight = nodes.Single(n => n.Row == 0 && n.Column == 1);
			var neighboursTR = graph.GetNeighbours(topRight).Cast<Node>().ToList();

			Assert.That(neighboursTR.Count, Is.EqualTo(2));
			Assert.IsTrue(neighboursTR.Any(n => n.Row == 0 && n.Column == 0));
			Assert.IsTrue(neighboursTR.Any(n => n.Row == 1 && n.Column == 1));

			var btmLeft = nodes.Single(n => n.Row == 1 && n.Column == 0);
			var neighboursBL = graph.GetNeighbours(btmLeft).Cast<Node>().ToList();

			Assert.That(neighboursBL.Count, Is.EqualTo(2));
			Assert.IsTrue(neighboursBL.Any(n => n.Row == 0 && n.Column == 0));
			Assert.IsTrue(neighboursBL.Any(n => n.Row == 1 && n.Column == 1));

			var btmRight = nodes.Single(n => n.Row == 1 && n.Column == 1);
			var neighboursBR = graph.GetNeighbours(btmRight).Cast<Node>().ToList();

			Assert.That(neighboursBR.Count, Is.EqualTo(2));
			Assert.IsTrue(neighboursBR.Any(n => n.Row == 0 && n.Column == 1));
			Assert.IsTrue(neighboursBR.Any(n => n.Row == 1 && n.Column == 0));

			Assert.That(graph.EdgeCount, Is.EqualTo(8));
		}

		[Test]
		public void CreatingGraphFromGrid_Again_ResetsPreviousGraph()
		{
			var graph = new Graph<Node>();

			int[][] grid1 =
			{
				[1, 0],
			};

			int[][] grid2 =
			{
				[1, 1],
				[0, 1]
			};

			graph.CreateGraphFromGrid(grid1);

			Assert.That(graph.NodeCount, Is.EqualTo(2));
			Assert.That(graph.EdgeCount, Is.EqualTo(0));

			graph.CreateGraphFromGrid(grid2);

			Assert.That(graph.NodeCount, Is.EqualTo(4));
			Assert.That(graph.EdgeCount, Is.EqualTo(4)); 

			var nodes = graph.Nodes.Cast<Node>().ToList();
			Assert.IsTrue(nodes.Any(n => n.Row == 0 && n.Column == 0));
			Assert.IsTrue(nodes.Any(n => n.Row == 0 && n.Column == 1));
			Assert.IsTrue(nodes.Any(n => n.Row == 1 && n.Column == 1));
		}

		[Test]
		public void CreatingGraphFromGrid_UnevenGrid_Throws()
		{
			var graph = new Graph<Node>();

			int[][] grid =
			{
				[1, 1, 1],
				[1, 1]
			};

			Assert.Throws<IndexOutOfRangeException>(() => graph.CreateGraphFromGrid(grid));
		}
	}
}
