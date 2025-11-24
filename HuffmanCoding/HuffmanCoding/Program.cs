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
				"  HuffmanCoding.ex decode <input_file> <output_file> \n" +
				"     Decodes a previously compressed file. \n\n" +
				"Example: \n" +
				"  HuffmanCoding.ex encode message.txt message.huf \n" +
				"  HuffmanCoding.ex decode message.huf message_decoded.txt \n";

			if (args.Length < 3)
			{
				Console.WriteLine(usage);
				return;
			}

			var mode = "decode";
			var inputFile = "Z:\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\output.huf";
			var outputFile = "Z:\\Desktop\\FH\\Master\\Semester1\\Advanced Algorithmics\\output2.txt";


			// encode
			// "Z:\Desktop\FH\Master\Semester1\Advanced Algorithmics\input.txt"
			// "Z:\Desktop\FH\Master\Semester1\Advanced Algorithmics\output.huf"

			if (!File.Exists(inputFile))
			{
				throw new FileNotFoundException("The input file you have provided does not exist.", inputFile);
			}

			HuffmanCoder huffman = new HuffmanCoder(inputFile, outputFile);

			if (mode.ToLower() == "decode")
			{
				Console.WriteLine("decoding");
				huffman.Decode();
			}
			else if (mode.ToLower() == "encode")
			{
				Console.WriteLine("encoding");
				huffman.Encode();
			}
			else
			{
				Console.WriteLine("Unknown Command");
				Console.WriteLine(usage);
			}
		}
	}
}