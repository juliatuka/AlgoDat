using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver_Test
{
	[TestFixture]
	public class SolverTest
	{
		private string CaptureConsoleOutput(Action action)
		{
			using var sw = new StringWriter();
			Console.SetOut(sw);

			action();

			Console.Out.Flush();
			return sw.ToString();
		}

		private static int[][] CloneGrid(int[][] g)
			=> g.Select(row => row.ToArray()).ToArray();

		private static int[][] EasyPuzzle() => new int[][]
		{
			new[] {0,7,8,0,6,0,4,0,0},
			new[] {4,2,9,1,8,5,7,6,3},
			new[] {0,0,0,0,7,4,0,0,8},
			new[] {8,0,0,0,0,6,0,0,7},
			new[] {7,0,0,0,0,0,3,0,6},
			new[] {1,0,6,7,0,0,0,0,4},
			new[] {0,8,0,0,3,0,0,7,2},
			new[] {0,5,7,6,0,0,0,0,0},
			new[] {0,0,0,0,0,7,0,0,9},
		};

		private static int[][] ContradictoryPuzzle() => new int[][]
	   {
			new[] {7,7,0,0,0,0,0,0,0}, // duplicate 7 in row
            new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
	   };

		private static int[][] SolvedPuzzle() => new int[][]
	   {
			new[] {5,3,4,6,7,8,9,1,2},
			new[] {6,7,2,1,9,5,3,4,8},
			new[] {1,9,8,3,4,2,5,6,7},
			new[] {8,5,9,7,6,1,4,2,3},
			new[] {4,2,6,8,5,3,7,9,1},
			new[] {7,1,3,9,2,4,8,5,6},
			new[] {9,6,1,5,3,7,2,8,4},
			new[] {2,8,7,4,1,9,6,3,5},
			new[] {3,4,5,2,8,6,1,7,9},
	   };

		private static int[][] UnsolvedPuzzle() => new int[][]
	   {
			new[] {0,1,2,3,4,5,6,7,8},
			new[] {9,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
			new[] {0,0,0,0,0,0,0,0,0},
	   };

		private static bool IsValidSolution(int[][] grid)
		{
			if (grid.Length != 9) return false;
			if (grid.Any(r => r.Length != 9)) return false;

			bool CheckGroup(int[] group)
			{
				// contains digits 1-9 exactly once
				var sorted = group.OrderBy(x => x).ToArray();
				for (int i = 0; i < 9; i++)
					if (sorted[i] != i + 1) return false;
				return true;
			}

			for (int r = 0; r < 9; r++)
				if (!CheckGroup(grid[r])) return false;

			for (int c = 0; c < 9; c++)
			{
				var col = new int[9];
				for (int r = 0; r < 9; r++) col[r] = grid[r][c];
				if (!CheckGroup(col)) return false;
			}

			for (int br = 0; br < 3; br++)
			{
				for (int bc = 0; bc < 3; bc++)
				{
					var box = new int[9];
					int k = 0;
					for (int r = br * 3; r < br * 3 + 3; r++)
						for (int c = bc * 3; c < bc * 3 + 3; c++)
							box[k++] = grid[r][c];

					if (!CheckGroup(box)) return false;
				}
			}

			return true;
		}

		[Test]
		public void CreateGraphFromSudokuGrid_BuildsGraph()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = EasyPuzzle();

			solver.CreateGraphFromSudokuGrid(grid);

			Assert.NotNull(solver.graph);
			Assert.That(solver.graph.AdjacencyList.Keys.Count, Is.EqualTo(81));
			foreach (var kvp in solver.graph.AdjacencyList)
				Assert.That(kvp.Value.Count, Is.EqualTo(20));
		}

		[Test]
		public void CreateGraphFromSudokuGrid_DuplicateCellsInInvalidePlace_Throws()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = ContradictoryPuzzle();

			Assert.Throws<InvalidDataException>(() => solver.CreateGraphFromSudokuGrid(grid));
		}

		[Test]
		public void ValidateGivenCells_ReturnsTrueForValidPuzzle()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = EasyPuzzle();

			solver.CreateGraphFromSudokuGrid(grid);
			Assert.True(solver.ValidateGivenCells(grid));
		}

		[Test]
		public void Solve_EasyPuzzle_ReturnsTrue_AndProducesValidSolution()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = EasyPuzzle();
			var original = CloneGrid(grid);

			solver.CreateGraphFromSudokuGrid(grid);

			bool ok = solver.Solve(grid);

			Assert.True(ok);
			Assert.True(IsValidSolution(grid));

			// Making sure the original entries were not changed
			for (int r = 0; r < 9; r++)
			{
				for (int c = 0; c < 9; c++)
				{
					if (original[r][c] != 0)
						Assert.That(original[r][c], Is.EqualTo(grid[r][c]));
				}
			}
		}

		[Test]
		public void Solve_AlreadySolvedPuzzle_ReturnsTrue_AndKeepsGridValid()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = SolvedPuzzle();
			var original = CloneGrid(grid);

			solver.CreateGraphFromSudokuGrid(grid);
			bool ok = solver.Solve(grid);

			Assert.True(ok);
			Assert.True(IsValidSolution(grid));

			for (int r = 0; r < 9; r++)
				for (int c = 0; c < 9; c++)
					Assert.That(original[r][c], Is.EqualTo(grid[r][c]));
		}

		[Test]
		public void PrintResult_WritesResultToConsole()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = EasyPuzzle();
			solver.CreateGraphFromSudokuGrid(grid);
			bool ok = solver.Solve(grid);
			Assert.True(ok);

			string output = CaptureConsoleOutput(() =>
			{
				solver.PrintResult();

			});


			Assert.True(output.Length > 0);
			Assert.True(output.Contains("Solved Sudoku"));
		}

		[Test]
		public void PrintResult_ColorsNull_PrintsNoSolutionMessage()
		{
			var solver = new SudokuSolver.SudokuSolver();

			string output = CaptureConsoleOutput(() =>
			{
				solver.PrintResult();

			});

			Assert.True(output.Length > 0);
			Assert.True(output.Contains("No solution to print."));
		}

		[Test]
		public void Solve_Unsolvable_Sudoku_ReturnsFalse()
		{
			var solver = new SudokuSolver.SudokuSolver();
			var grid = UnsolvedPuzzle();

			solver.CreateGraphFromSudokuGrid(grid);

			bool solved = solver.Solve(grid);
			Assert.False(solved);
		}
	}
}
