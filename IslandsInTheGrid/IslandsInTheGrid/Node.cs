using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IslandsInTheGrid
{
	public class Node
	{
		public Node(int row, int col, bool isLand)
		{
			Row = row;
			Column = col;
			IsLand = isLand;
			IslandFlag = 0;
		}

		public int Row { get; set; }

		public int Column { get; set; }

		public bool IsLand { get; set; }

		public int IslandFlag { get; set; }
	}
}
