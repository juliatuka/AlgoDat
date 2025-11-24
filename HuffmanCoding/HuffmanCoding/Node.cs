using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding
{
	public class Node
	{
		private char character;
		private int frequency;
		private Node left;
		private Node right;	

		public Node(int frequency)
		{
			this.Frequency = frequency;
		}

		public char Character { get; set; }

		public int Frequency { get; set; }
		public Node Left { get; set; }
		public Node Right { get; set; }
	}
}
