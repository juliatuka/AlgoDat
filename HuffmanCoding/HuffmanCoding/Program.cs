using System;

namespace HuffmanCoding
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CommandLineInterface(args);
		}

		public static void CommandLineInterface(string[] args)
		{
			string usage =
				"Huffman Coding Tool Usage: \n" +
				"  HuffmanCoding.exe encode <input_file> <output_file> \n" +
				"     Encodes the given file and writes the compressed output. \n\n" +
				"  HuffmanCoding.exe decode <input_file> <output_file> \n" +
				"     Decodes a previously compressed file. \n\n" +
				"Example: \n" +
				"  HuffmanCoding.exe encode message.txt message.huf \n" +
				"  HuffmanCoding.exe decode message.huf message_decoded.txt \n";

			if (args.Length != 3)
			{
				Console.WriteLine("Error: Wrong number of arguments.\n");
				Console.WriteLine(usage);
				return;
			}

			string mode = args[0].ToLower();
			string inputFile = args[1];
			string outputFile = args[2];

			//string mode = "decode";
			//string inputFile = "Z:\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\output.huf";
			//string outputFile = "Z:\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\output2.txt";

			//var mode = "encode";
			//var inputFile = "Z:\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\input.txt";
			//var outputFile = "Z:\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\output.huf";


			if (!File.Exists(inputFile))
			{
				Console.WriteLine($"Error: The input file ({inputFile}) you have provided does not exist.");
			}

			try
			{
				HuffmanCoder huffman = new HuffmanCoder(inputFile, outputFile);

				if (mode.ToLower() == "decode")
				{
					Console.WriteLine("Decoding...");
					huffman.Decode();
					Console.WriteLine("Decoding completed.");
				}
				else if (mode.ToLower() == "encode")
				{
					Console.WriteLine("Encoding...");
					huffman.Encode();
					Console.WriteLine("Encoding completed.");
				}
				else
				{
					Console.WriteLine($"Error: Unknown Command '{mode}'.\n");
					Console.WriteLine(usage);
				}
			}
			catch (InvalidDataException ex)
			{
				Console.WriteLine($"Huffman file error: {ex.Message}");
			}
			catch (IOException ex)
			{
				Console.WriteLine($"I/O error: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected error: {ex.Message}");
			}
		}
	}
}