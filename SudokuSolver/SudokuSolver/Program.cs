
using System.Text.Json;
using System.Xml.Linq;

namespace SudokuSolver
{
	public class Program
	{
		static void Main(string[] args)
		{
			// RunCLI(args[0]);
			RunCLI("C:\\Mac\\Home\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\AlgoDat\\SudokuSolver\\SudokuSolver\\easySudoku.json");
		}

		static string usage = "		Please provide a json file containing a 9x9 grid containing a Sudoku Puzzle, \n" +
					"Example:\n\n" +
					"{\r\n  \"sudoku\": [\r\n    " +
			"[0, 7, 8, 0, 6, 0, 4, 0, 0],\r\n    " +
			"[4, 2, 9, 1, 8, 5, 7, 6, 3],\r\n    " +
			"[0, 0, 0, 0, 7, 4, 0, 0, 8],\r\n    " +
			"[8, 0, 0, 0, 0, 6, 0, 0, 7],\r\n    " +
			"[7, 0, 0, 0, 0, 0, 3, 0, 6],\r\n    " +
			"[1, 0, 6, 7, 0, 0, 0, 0, 4],\r\n    " +
			"[0, 8, 0, 0, 3, 0, 0, 7, 2],\r\n    " +
			"[0, 5, 7, 6, 0, 0, 0, 0, 0],\r\n    " +
			"[0, 0, 0, 0, 0, 7, 0, 0, 9]\r\n  ]" +
			"}";

		public static bool RunCLI(string file)
		{
			if (file.Length <= 0)
			{
				Console.WriteLine(usage);
				return false;
			}

			if (!ProcessSudokuFile(file))
			{
				Console.WriteLine(usage);
				return false;
			}

			return true;
		}

		public static bool ProcessSudokuFile(string file)
		{
			if (!File.Exists(file))
			{
				Console.WriteLine("The file does not exist.");
				return false;
			}

			Wrapper? jsongrid;

			try
			{
				using var stream = new FileStream(file, FileMode.Open);
				using var reader = new StreamReader(stream);
				string json = reader.ReadToEnd();

				jsongrid = JsonSerializer.Deserialize<Wrapper>(json);
			}
			catch (JsonException ex)
			{
				Console.WriteLine($"Error: file does not contain valid sudoku JSON. {ex.Message}");
				return false;
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error reading file: {ex.Message}");
				return false;
			}

			if (jsongrid?.sudoku == null)
			{
				Console.WriteLine("Error: JSON does not contain a 'sudoku' property. Or property is null.");
				return false;
			}

			int[][] sudokuGrid = jsongrid.sudoku;

			if (sudokuGrid.Length == 0)
			{
				Console.WriteLine("The provided sudoku object is empty");
				return false;
			}

			SudokuSolver solver = new SudokuSolver();
			solver.CreateGraphFromSudokuGrid(sudokuGrid);

			bool solved = solver.Solve(sudokuGrid);
			if (!solved)
			{
				Console.WriteLine("No solution found."); 
				return false;
			}

			solver.PrintResult();

			return true;
		}
	}
}