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
			PriorityQueue<Node, int> sortedQueue = new PriorityQueue<Node, int>();

			foreach (var item in frequTable)
			{
				Node node = new Node(item.Value);
				node.Character = item.Key;
				sortedQueue.Enqueue(node, item.Value);
			}

			Console.WriteLine(sortedQueue.Count);

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
			if (node.Character != 0)
			{
				codeTable[node.Character] = prefix;
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
			string result = "";
			Node node = root;

			foreach (var b in bits)
			{
				if (b == '0')
				{
					node = node.Left;
				}
				else
				{
					node = node.Right;
				}


				if (node.Character != 0)
				{
					result += node.Character;
					node = root;
				}
			}

			return result;
		}

		public void Decode()
		{
			using (FileStream stream = new FileStream(inputFile, FileMode.Open))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					if (reader.ReadLine() != "HF")
					{
						return; // invalide file
					}

					var table = new Dictionary<char, int>();
					int frequencyCount = int.Parse(reader.ReadLine());

					for (int i = 0; i < frequencyCount; i++)
					{
						var frequ = reader.ReadLine().Split(" ");

						table.Add(char.Parse(frequ[0]), int.Parse(frequ[1]));
					}

					int originalLength = int.Parse(reader.ReadLine());
					string bits = reader.ReadLine();

					Node root = BuildHuffmanTree(table);
					string result = DecodeBits(inputFile, root);
				}

				// write to output file 
			}
		}

		public void Encode()
		{
			string text = File.ReadAllText(inputFile, Encoding.UTF8);

			var table = BuildFrequencyTable(text);
			var root = BuildHuffmanTree(table);
			var codeTable = BuildCodeTable(root);

			string encoded = EncodeText(text, codeTable);

			WriteToFile(encoded, table, text.Length);
		}

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

						writer.WriteLine($"{symbol} {frequency}");
					}

					writer.WriteLine(originalTextLength);

					writer.WriteLine(encoded);
				}
			}
		}

		//private Node root;
	}
}
