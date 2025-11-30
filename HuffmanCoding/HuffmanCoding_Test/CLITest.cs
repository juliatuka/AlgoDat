using HuffmanCoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding_Test
{
	[TestFixture]
	public class CLITest
	{
		private string tempDir;
		private string inputPath;
		private string encodePath;
		private string decodePath;

		[SetUp]
		public void Setup()
		{
			tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDir);

			inputPath = Path.Combine(tempDir, "input.txt");
			encodePath = Path.Combine(tempDir, "encoded.huf");
			decodePath = Path.Combine(tempDir, "decoded.txt");
		}

		[TearDown]
		public void Cleanup()
		{
			if (Directory.Exists(tempDir))
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		// Helper to capture Console.Out for the time of the action.
		// Returns the output as string.
		private string CaptureConsoleOutput(Action action)
		{
			var originalOut = Console.Out;
			var stringBuilder = new StringBuilder();
			using (var writer = new StringWriter(stringBuilder))
			{
				try
				{
					Console.SetOut(writer);
					action();
				}
				finally
				{
					Console.SetOut(originalOut);
				}
			}
			return stringBuilder.ToString();
		}

		[Test]
		public void CommandLinInterfacee_Wrong_Number_Of_Arguments_Prints_ErrorAndUsage()
		{
			string[] args = Array.Empty<string>();

			string output = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(args);
			});

			StringAssert.Contains("Error: Wrong number of arguments.", output);
			StringAssert.Contains("Huffman Coding Tool Usage:", output);
		}

		[Test]
		public void CommandLineInterface_Unknown_Command_Prints_ErrorAndUsage()
		{
			File.WriteAllText(inputPath, "some content", Encoding.UTF8);
			string[] args = { "compress", inputPath, encodePath };

			string output = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(args);
			});

			StringAssert.Contains("Error: Unknown Command 'compress'.", output);
			StringAssert.Contains("Huffman Coding Tool Usage:", output);
		}

		[Test]
		public void CommandLineInterface_Encoding_NonExisting_Input_Prints_ErrorAndIOError()
		{
			string missingInput = Path.Combine(tempDir, "does_not_exist.txt");
			string[] args = { "encode", missingInput, encodePath };

			string output = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(args);
			});

			StringAssert.Contains($"Error: The input file ({missingInput}) you have provided does not exist.", output);
			StringAssert.Contains("I/O error: I/O error while encoing file", output);
		}

		[Test]
		public void CommandLineInterface_Decode_NonExisting_Input_Prints_ErrorAndIoError()
		{
			string missingInput = Path.Combine(tempDir, "does_not_exist.huf");
			string[] args = { "decode", missingInput, decodePath };

			string output = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(args);
			});

			StringAssert.Contains($"Error: The input file ({missingInput}) you have provided does not exist.", output);
			StringAssert.Contains("I/O error: I/O error while decoding file", output);
		}

		[Test]
		public void CommandLineInterface_Encoding_ValidInput_Writes_EncodedFile_And_PrintsMessages()
		{
			string original = "Huffman CLI encode test.";
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			string[] args = { "encode", inputPath, encodePath };

			string output = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(args);
			});

			StringAssert.Contains("Encoding...", output);
			StringAssert.Contains("Encoding completed.", output);

			Assert.That(File.Exists(encodePath));
			Assert.That(new FileInfo(encodePath).Length, Is.GreaterThan(0), "Encoded file should not be empty.");
		}

		[Test]
		public void CommandLineInterface_Decode_ValidFile_RestoresOriginalTextAndPrintsMessages()
		{
			string original = "Huffman CLI full round-trip test.";
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			string[] encodeArgs = { "encode", inputPath, encodePath };
			CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(encodeArgs);
			});

			Assert.That(File.Exists(encodePath));


			string[] decodeArgs = { "decode", encodePath, decodePath };
			string decodeOutput = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(decodeArgs);
			});

			StringAssert.Contains("Decoding...", decodeOutput);
			StringAssert.Contains("Decoding completed.", decodeOutput);

			string decoded = File.ReadAllText(decodePath, Encoding.UTF8);
			Assert.That(original, Is.EqualTo(decoded));
		}

		[Test]
		public void CommandLineInterface_ModeIsCaseInsensitive()
		{
			string original = "Case-insensitive mode test.";
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			string[] encodeArgs = { "EnCoDe", inputPath, encodePath };
			string encodeOutput = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(encodeArgs);
			});

			Assert.That(File.Exists(encodePath));
			StringAssert.Contains("Encoding...", encodeOutput);
			StringAssert.Contains("Encoding completed.", encodeOutput);

			string[] decodeArgs = { "DeCoDe", encodePath, decodePath };
			string decodeOutput = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(decodeArgs);
			});

			StringAssert.Contains("Decoding...", decodeOutput);
			StringAssert.Contains("Decoding completed.", decodeOutput);

			string decoded = File.ReadAllText(decodePath, Encoding.UTF8);
			Assert.That(original, Is.EqualTo(decoded));
		}

		[Test]
		public void CommandLineInterface_Decode_InvalidHuffmanFile_PrintsHuffmanFileError()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"NOT_HF",
				"0"
			});

			string[] args = { "decode", encodePath, decodePath };

			string output = CaptureConsoleOutput(() =>
			{
				Program.CommandLineInterface(args);
			});

			StringAssert.Contains("Huffman file error:", output);
			StringAssert.Contains("missing 'HF' header", output);
		}
	}
}
