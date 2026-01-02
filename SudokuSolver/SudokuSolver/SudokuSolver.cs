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
		private bool[] fixedCell;

		public SudokuSolver()
		{
		}

		public void CreateGraphFromSudokuGrid(int[][] grid)
		{
			graph = new Graph<Node>();

			graph.CreateGraphFromSudokuGrid(grid);

			InitalizeColors(grid);

			if (!ValidateGivenCells(grid))
				throw new InvalidDataException("Invalid Sudoku: given cells conflict.");
		}

		private int Id(int r, int c) => r * 9 + c;

		public void InitalizeColors(int[][] grid)
		{
			colors = new int[81];
			fixedCell = new bool[81];

			for (int r = 0; r < grid.Length; r++)
			{
				for (int c = 0; c < grid[r].Length; c++)
				{
					int id = Id(r, c);
					int val = grid[r][c];

					colors[id] = val;
					fixedCell[id] = (val != 0);
				}
			}
		}

		public bool ValidateGivenCells(int[][] grid)
		{
			int[] colors = new int[81];
			for (int r = 0; r < 9; r++)
			{
				for (int c = 0; c < 9; c++)
				{
					colors[r * 9 + c] = grid[r][c];
				}
			}

			foreach (var kvp in graph.AdjacencyList)
			{
				int id = kvp.Key;
				int val = colors[id];
				if (val == 0) continue;

				foreach (var edge in kvp.Value)
				{
					if (colors[edge.Target] == 0) continue;

					if (val == colors[edge.Target]) // meaning the initial given cells make the sudoku unsolvable
						return false;
				}
			}

			return true;
		}

		public bool Solve(int[][] grid)
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

				foreach (var edge in graph.AdjacencyList[index])
					neighborColors[edge.Target].Add(colors[index]);
			}

			bool ok = Backtrack(neighborColors);

			if (ok)
			{
				// write "colors" back into grid
				for (int r = 0; r < 9; r++)
					for (int c = 0; c < 9; c++)
						grid[r][c] = colors[Id(r, c)];
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

				// assign "color"
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

				// DSatur
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
			if (colors == null)
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

					int id = r * 9 + c;
					Console.Write(colors[id] + " ");
				}

				Console.WriteLine();
			}
		}
	}
}
