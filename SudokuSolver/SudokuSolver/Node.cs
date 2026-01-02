using System.Drawing;

namespace SudokuSolver
{
	public class Node
	{
		public Node(int row, int col) 
		{ 
			Row = row;
			Col = col;
		}

		public int Row { get; set; }

		public int Col { get; set; }
	}
}