using IslandsInTheGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandsInTheGrid_Test
{
	[TestFixture]
	public class ProcessGridFile_Test
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
		public void ProcessGridFile_File_DoesNotExist_ResturnsFalse()
		{
			string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("The file does not exist", output);
		}

		[Test]
		public void ProcessGridFile_Invalide_Json_ReturnsFalse()
		{
			string path = CreateTempFile("this is not json.");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("file does not contain valid grid JSON", output);
		}

		[Test]
		public void ProcessGridFile_Json_DoesNotContain_Grid_Property_ReturnsFalse()
		{
			string path = CreateTempFile(@"{  ""notGrid"": [1,2,3]  }");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("JSON does not contain a", output);
		}

		[Test]
		public void ProcessGridFile_Json_Contains_NullGrid_ReturnsFalse()
		{
			string path = CreateTempFile(@"{  ""grid"": null  }");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("Or grid is null", output);
		}

		[Test]
		public void ProcessGridFile_Json_Contains_EmptyGrid_ReturnsTrue_ZeroIslands()
		{
			string path = CreateTempFile(@"{  ""grid"": []  }");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsTrue(result);
			});

			StringAssert.Contains("The provided grid is empty. Number of Islands: 0", output);
		}

		[Test]
		public void ProcessGridFile_Grid_Without_Islands_ReturnsTrue_ZeroIslands()
		{
			string path = CreateTempFile(@"{  
""grid"": [
    [ 0, 0, 0 ],
    [ 0, 0, 0 ],
    [ 0, 0, 0 ]
  ]
}");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsTrue(result);
			});

			StringAssert.Contains("Number of Islands: 0", output);
			StringAssert.Contains("No Islands found.", output);
		}

		[Test]
		public void ProcessGridFile_Grid_With_Islands_ReturnsTrue_PrintsCorrectCount()
		{
			string path = CreateTempFile(@"{  
""grid"": [
    [ 1, 1, 0, 0 ],
    [ 1, 0, 0, 1 ],
    [ 0, 0, 1, 1 ],
    [ 1, 1, 0, 1 ]
  ]
}");

			string output = CaptureConsoleOutput(() =>
			{
				bool result = Program.ProcessGridFile(path);
				Assert.IsTrue(result);
			});

			StringAssert.Contains("Number of Islands: 3", output);
			StringAssert.Contains("Smallest Island size: 2", output);
			StringAssert.Contains("Largest Island size: 4", output);
		}

		[Test]
		public void RunCLI_PrintsUsage_When_FileDoesNotExist()
		{
			string nonExistingFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

			string consoleOutput = CaptureConsoleOutput(() =>
			{
				bool result = Program.RunCLI(nonExistingFile);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("The file does not exist.", consoleOutput);
			StringAssert.Contains("Please provide a json file containing a 2D grid", consoleOutput);
		}


		[Test]
		public void RunCli_NotPrintingUsage_WhenFileIsValid()
		{
			string json = """
            {
              "grid": [
                [ 1, 1, 0, 0 ],
                [ 1, 1, 0, 0 ],
                [ 0, 0, 1, 1 ],
                [ 1, 1, 0, 1 ]
              ]
            }
            """;

			string path = CreateTempFile(json);

			try
			{
				bool result = false;
				string output = CaptureConsoleOutput(() =>
				{
					result = Program.RunCLI(path);
				});

				Assert.IsTrue(result, "RunCli should return true for a valid file.");
				StringAssert.DoesNotContain("Please provide a json file containing a 2D grid", output);
				StringAssert.Contains("Number of Islands:", output); 
			}
			finally
			{
				File.Delete(path);
			}
		}

		[Test]
		public void RunCLI_PrintsUsage_When_FilePathEmpty()
		{
			string consoleOutput = CaptureConsoleOutput(() =>
			{
				bool result = Program.RunCLI(string.Empty);
				Assert.IsFalse(result);
			});

			StringAssert.Contains("Please provide a json file containing a 2D grid", consoleOutput);
		}
	}
}
