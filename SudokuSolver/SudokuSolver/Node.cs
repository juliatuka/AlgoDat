using System.Drawing;

namespace SudokuSolver
{
	public class Node
	{
		public Node(int row, int col, int val) 
		{ 
			Row = row;
			Col = col;
			Value = val;
		}

		public int Row { get; set; }

		public int Col { get; set; }

		public int Value { get; set; }
	}
}