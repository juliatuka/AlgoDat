using SudokuSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver_Test
{
	[TestFixture]
	public class CLITest
	{
		private string CaptureConsoleOutput(Action action)
		{
			using var sw = new StringWriter();
			Console.SetOut(sw);

			action();

			Console.Out.Flush();
			return sw.ToString();
		}

		private string CreateTempFile(string contents)
		{
			string path = Path.GetTempFileName();
			File.WriteAllText(path, contents);
			return path;
		}

		[Test]
		public void RunCLI_EmptyPath_PrintsUsage_ReturnsFalse()
		{
			string nonExistingFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

			string consoleOutput = CaptureConsoleOutput(() =>
			{
				bool result = Program.RunCLI(nonExistingFile);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("The file does not exist.", consoleOutput);
			StringAssert.Contains("Please provide a json file containing a 9x9 grid ", consoleOutput);
		}

		[Test]
		public void RunCLI_FilePathEmpty_PrintsUsage()
		{
			string consoleOutput = CaptureConsoleOutput(() =>
			{
				bool result = Program.RunCLI(string.Empty);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("Please provide a json file containing a 9x9 grid", consoleOutput);
		}

		[Test]
		public void ProcessSudokuFile_FileDoesNotExists_ReturnsFalse()
		{
			string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessSudokuFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("The file does not exist", output);
		}

		[Test]
		public void ProcessSudokuFile_Invalide_Json_ReturnsFalse()
		{
			string path = CreateTempFile("this is not json.");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessSudokuFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("file does not contain valid sudoku JSON", output);
		}

		[Test]
		public void ProcessSudokuFile_Json_Missing_Sudoku_Property_ReturnsFalse()
		{
			string path = CreateTempFile(@"{  ""notSudoku"": [1,2,3]  }");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessSudokuFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("JSON does not contain a", output);
		}

		[Test]
		public void ProcessSudokuFile_Json_Contains_NullGrid_ReturnsFalse()
		{
			string path = CreateTempFile(@"{  ""sudoku"": null  }");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessSudokuFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("Or property is null", output);
		}

		[Test]
		public void ProcessSudokuFile_Json_Contains_EmptyGrid_ReturnsFalse()
		{
			string path = CreateTempFile(@"{  ""sudoku"": []  }");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessSudokuFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("The provided sudoku object is empty", output);
		}

		[Test]
		public void ProcessSudokuFile_ValidPuzzle_ReturnsTrue()
		{
			string validSudokuJson =
@"{
  ""sudoku"": [
    [0,7,8,0,6,0,4,0,0],
    [4,2,9,1,8,5,7,6,3],
    [0,0,0,0,7,4,0,0,8],
    [8,0,0,0,0,6,0,0,7],
    [7,0,0,0,0,0,3,0,6],
    [1,0,6,7,0,0,0,0,4],
    [0,8,0,0,3,0,0,7,2],
    [0,5,7,6,0,0,0,0,0],
    [0,0,0,0,0,7,0,0,9]
  ]
}";
			string path = CreateTempFile(validSudokuJson);

			try
			{
				string output = CaptureConsoleOutput(() =>
				{
					bool ok = Program.ProcessSudokuFile(path);
					Assert.IsTrue(ok);
				});

				StringAssert.DoesNotContain("No solution found.", output);
				StringAssert.Contains("Solved", output);
			}
			finally
			{
				if (File.Exists(path)) File.Delete(path);
			}
		}

		[Test]
		public void ProcessSudokuFile_WrongGivenCells_ThrowsInvalidSudokuException()
		{
			string badSudokuJson =
@"{
  ""sudoku"": [
    [7,7,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0]
  ]
}";
			string path = CreateTempFile(badSudokuJson);

			try
			{

				string output = CaptureConsoleOutput(() =>
				{
					Assert.Throws<InvalidDataException>(() => Program.ProcessSudokuFile(path), "Invalid Sudoku: given cells conflict.");
				});

			}
			finally
			{
				if (File.Exists(path)) File.Delete(path);
			}
		}

		[Test]
		public void RunCLI_Valid_SudokuFile_ReturnsTrue()
		{
			string json =
@"{
  ""sudoku"": [
    [0,7,8,0,6,0,4,0,0],
    [4,2,9,1,8,5,7,6,3],
    [0,0,0,0,7,4,0,0,8],
    [8,0,0,0,0,6,0,0,7],
    [7,0,0,0,0,0,3,0,6],
    [1,0,6,7,0,0,0,0,4],
    [0,8,0,0,3,0,0,7,2],
    [0,5,7,6,0,0,0,0,0],
    [0,0,0,0,0,7,0,0,9]
  ]
}";

			string path = CreateTempFile(json);

			try
			{
				string output = CaptureConsoleOutput(() =>
				{
					bool result = Program.RunCLI(path);
					Assert.True(result);
				});
			}
			finally
			{
				if (File.Exists(path))
					File.Delete(path);
			}
		}

		[Test]
		public void RunCLI_InValidSudoku_ReturnsFalse()
		{
			string json =
@"{
  ""sudoku"": [
    [0,1,2,3,4,5,6,7,8],
    [9,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0],
    [0,0,0,0,0,0,0,0,0]
  ]
}";
			string path = CreateTempFile(json);

			try
			{
				string output = CaptureConsoleOutput(() =>
				{
					bool result = Program.RunCLI(path);
					Assert.False(result);
				});

				StringAssert.Contains("No solution found.", output);
			}
			finally
			{
				if (File.Exists(path))
					File.Delete(path);
			}
		}
	}
}
