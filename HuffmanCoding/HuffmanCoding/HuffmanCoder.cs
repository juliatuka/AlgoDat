using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HuffmanCoding
{
	public class HuffmanCoder
	{
		private string inputFile;
		private string outputFile;

		public HuffmanCoder(string inputFile, string outputFile)
		{
			this.inputFile = inputFile;
			this.outputFile = outputFile;
		}

		private Dictionary<char, int> BuildFrequencyTable(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input), "Input text must not be null.");
			}

			if (input.Length == 0)
			{
				throw new InvalidOperationException("Cannot build a frequency table from an empty input.");
			}

			Dictionary<char, int> frequencyTable = new Dictionary<char, int>();

			for (int i = 0; i < input.Length; i++)
			{
				if (frequencyTable.TryGetValue(input[i], out int frq))
				{
					frequencyTable[input[i]] = ++frq;
				}
				else
				{
					frequencyTable.Add(input[i], 1);
				}
			}

			return frequencyTable;
		}

		private Node BuildHuffmanTree(Dictionary<char, int> frequTable)
		{
			if (frequTable == null)
			{
				throw new ArgumentException(nameof(frequTable), "Frequency table must not be null.");
			}

			if (frequTable.Count == 0)
			{
				throw new InvalidOperationException("Cannot build Huffman tree from empty frequency table.");
			}

			PriorityQueue<Node, int> sortedQueue = new PriorityQueue<Node, int>();

			foreach (var item in frequTable)
			{
				if (item.Value <= 0)
				{
					throw new InvalidDataException($"Invalid frequency {item.Value} for symbol '{item.Key}'");
				}

				Node node = new Node(item.Value);
				node.Character = item.Key;
				sortedQueue.Enqueue(node, item.Value);
			}

			while (sortedQueue.Count > 1)
			{
				Node node1 = sortedQueue.Dequeue();
				Node node2 = sortedQueue.Dequeue();

				var frequency = node1.Frequency + node2.Frequency;
				Node parent = new Node(frequency);
				parent.Left = node1;
				parent.Right = node2;

				sortedQueue.Enqueue(parent, frequency);
			}

			return sortedQueue.Dequeue();
		}

		private Dictionary<char, string> BuildCodeTable(Node root)
		{
			Dictionary<char, string> codeTable = new Dictionary<char, string>();
			Traverse(root, "", codeTable);
			return codeTable;
		}

		private void Traverse(Node node, string prefix, Dictionary<char, string> codeTable)
		{
			if (node == null)
				throw new InvalidDataException("Invalid Huffman tree: encountered null node while traversing.");

			if (node.Character != 0 && node.Left == null && node.Right == null)
			{
				codeTable[node.Character] = prefix.Length == 0 ? "0" : prefix;
				return;
			}

			Traverse(node.Left, prefix + "0", codeTable);
			Traverse(node.Right, prefix + "1", codeTable);
		}

		private string EncodeText(string text, Dictionary<char, string> codeTable)
		{
			string bits = "";

			foreach (var c in text)
				bits += codeTable[c];

			return bits;
		}

		private string DecodeBits(string bits, Node root)
		{
			if (root == null)
			{
				throw new ArgumentNullException(nameof(root), "The root Node must not be null.");
			}

			string result = "";
			Node node = root;

			foreach (var b in bits)
			{
				if (b == '0')
				{
					node = node.Left ?? throw new InvalidDataException("Invalid bitstream: tried to go left on a null node.");
				}
				else if (b == '1')
				{
					node = node.Right ?? throw new InvalidDataException("Invalid bitstream: tried to go right on a null node.");
				}
				else
				{
					throw new InvalidDataException($"Invalid bit '{b}' in encoded data. Only '0' and '1' are allowed.");
				}

				if (node.Character != 0) // is leaf
				{
					result += node.Character;
					node = root;
				}
			}

			return result;
		}

		public void Decode()
		{
			try
			{
				string decoded = DecodeFromFile(inputFile);

				WriteToFile(decoded);
			}
			catch (IOException ex)
			{
				throw new IOException($"I/O error while decoding file '{inputFile}'.", ex);
			}
		}

		private void WriteToFile(string text)
		{
			using (FileStream stream = new FileStream(outputFile, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.Write(text);
				}
			}
		}

		private string DecodeFromFile(string inputFile)
		{
			using (FileStream stream = new FileStream(inputFile, FileMode.Open))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					string header = reader.ReadLine();
					if (header != "HF")
					{
						throw new InvalidDataException("Invalide Huffman file: missing 'HF' header.");
					}

					if (!int.TryParse(reader.ReadLine(), out int frequencyCount))
					{
						throw new InvalidDataException("Invalid Huffman file: could not read symbol count.");
					}

					var table = new Dictionary<char, int>();

					for (int i = 0; i < frequencyCount; i++)
					{
						var line = reader.ReadLine();

						if (line == null)
						{
							throw new InvalidDataException($"Unexpected end of file in frequency table at entry {i}.");
						}

						var split = line.Split(',');

						if (split.Length != 2)
						{
							throw new InvalidDataException($"Invalid frequency line format: '{line}'");
						}

						string symbolPart = split[0].Trim();

						if (!int.TryParse(symbolPart, out int symbolCode))
						{
							throw new InvalidDataException($"Invalid symbol code value: '{symbolPart}'");
						}
						char symbol = (char)symbolCode;

						string frequPart = split[1].Trim();

						if (!int.TryParse(frequPart, out int frequency))
						{
							throw new InvalidDataException($"Invalid frequency value: '{frequPart}'");
						}

						table.Add(symbol, frequency);
					}


					if (!int.TryParse(reader.ReadLine(), out int originalLength))
					{
						throw new InvalidDataException("Invalid original length in header.");
					}

					string textbits = reader.ReadLine();

					if (textbits == null)
					{
						throw new InvalidDataException("Missing encoded bitstring in file.");
					}

					Node root = BuildHuffmanTree(table);
					string result = DecodeBits(textbits, root);

					if (result.Length != originalLength)
					{
						throw new InvalidDataException($"Decoded data length ({result.Length}) does not match expected original length ({originalLength}). File may be corrupted!");
					}

					return result;
				}
			}
		}

		public void Encode()
		{
			try
			{
				string text = File.ReadAllText(inputFile, Encoding.UTF8);

				var table = BuildFrequencyTable(text);
				var root = BuildHuffmanTree(table);
				var codeTable = BuildCodeTable(root);

				string encoded = EncodeText(text, codeTable);

				WriteToFile(encoded, table, text.Length);
			}
			catch (IOException ex)
			{
				throw new IOException($"I/O error while encoing file '{inputFile}'.", ex);
			}
		}

		// Huffman File format (.huf) header:
		// HF
		// <symbolCount>
		// <symbolAsInt>,<frequency>
		// ...
		// <originalLength>
		// <encodedBits>
		private void WriteToFile(string encoded, Dictionary<char, int> frequTable, int originalTextLength)
		{
			using (FileStream stream = new FileStream(outputFile, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine("HF");

					writer.WriteLine(frequTable.Count);

					foreach (var item in frequTable)
					{
						char symbol = item.Key;
						int frequency = item.Value;

						string message = $"{(int)symbol},{frequency}";
						writer.WriteLine(message);
					}

					writer.WriteLine(originalTextLength);

					writer.WriteLine(encoded);
				}
			}
		}
	}
}
