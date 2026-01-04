using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
	public class SudokuSolver
	{
		public Graph<Node> graph;
		private int[] colors;
		private int[][] grid;

		public SudokuSolver(int[][] sudokuGrid)
		{
			grid = sudokuGrid;
		}

		public Graph<Node> CreateGraphFromSudokuGrid()
		{
			graph = new Graph<Node>();

			graph.CreateGraphFromSudokuGrid(grid);

			InitalizeColors(graph);

			return graph;
		}

		private int Id(int r, int c) => r * 9 + c;

		public void InitalizeColors(Graph<Node> graph)
		{
			colors = new int[81];

			foreach (Node node in graph.Nodes)
			{
				int id = Id(node.Row, node.Col);
				colors[id] = node.Value;
			}
		}

		public bool ValidateGivenCells(Graph<Node> graph)
		{ 
			foreach (var kvp in graph.AdjacencyList)
			{
				int id = kvp.Key;
				int val = colors[id];
				if (val == 0) continue;

				foreach (var edge in kvp.Value)
				{
					if (colors[edge.Target] == 0) continue;

					// meaning the initial given cells make the sudoku unsolvable
					if (val == colors[edge.Target])
						return false;
				}
			}

			return true;
		}

		private void ApplyResultsToGrid(int[] colors)
		{
			// writing colors back into the grid
			for (int r = 0; r < 9; r++)
				for (int c = 0; c < 9; c++)
					grid[r][c] = colors[Id(r, c)];
		}

		public bool Solve()
		{
			// Build neighborColors from the current colors[] assignment (givens)
			var neighborColors = new HashSet<int>[81];
			for (int i = 0; i < 81; i++)
			{
				neighborColors[i] = new HashSet<int>();
			}

			for (int index = 0; index < 81; index++)
			{
				if (colors[index] == 0) continue;

				// Add the "color" of the current index to all graph neighbors to mark this color as forbidden for them
				foreach (var edge in graph.AdjacencyList[index])
					neighborColors[edge.Target].Add(colors[index]);
			}

			bool ok = Backtrack(neighborColors);

			if (ok)
			{
				ApplyResultsToGrid(colors);
			}

			return ok;
		}

		private bool Backtrack(HashSet<int>[] neighborColors)
		{
			int index = SelectNextCell(neighborColors);
			if (index == -1)
				return true;

			for (int digit = 1; digit <= 9; digit++)
			{
				if (neighborColors[index].Contains(digit)) continue;

				colors[index] = digit;

				var changed = new List<int>();
				foreach (var edge in graph.AdjacencyList[index])
				{
					int v = edge.Target;
					if (colors[v] == 0 && neighborColors[v].Add(digit))
						changed.Add(v);
				}

				if (Backtrack(neighborColors)) return true;

				foreach (int v in changed) neighborColors[v].Remove(digit);
				colors[index] = 0;
			}

			return false;
		}

		private int SelectNextCell(HashSet<int>[] neighborColors)
		{
			int bestIndex = -1;
			int bestSaturation = -1;
			int bestNumOfNeigh = -1;

			for (int index = 0; index < 81; index++)
			{
				if (colors[index] != 0) continue;

				int saturation = neighborColors[index].Count;
				int numOfNeigh = graph.AdjacencyList.TryGetValue(index, out var edges) ? edges.Count : 0;

				if (saturation > bestSaturation ||
					(saturation == bestSaturation && numOfNeigh > bestNumOfNeigh) ||
					(saturation == bestSaturation && numOfNeigh == bestNumOfNeigh && (bestIndex == -1 || index < bestIndex)))
				{
					bestIndex = index;
					bestSaturation = saturation;
					bestNumOfNeigh = numOfNeigh;
				}
			}

			return bestIndex;
		}

		public void PrintResult()
		{
			if (grid == null)
			{
				Console.WriteLine("No solution to print.");
				return;
			}

			Console.WriteLine("Solved Sudoku:\n");

			for (int r = 0; r < 9; r++)
			{
				if (r % 3 == 0 && r != 0)
					Console.WriteLine("------+-------+------");

				for (int c = 0; c < 9; c++)
				{
					if (c % 3 == 0 && c != 0)
						Console.Write("| ");

					Console.Write(grid[r][c] + " ");
				}

				Console.WriteLine();
			}
		}
	}
}
