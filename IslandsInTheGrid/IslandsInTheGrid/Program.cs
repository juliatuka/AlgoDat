using System;
using System.Text.Json;

namespace IslandsInTheGrid
{
	public class Program
	{
		public static void Main(string[] args)
		{
			RunCLI(args[0]);	
		}

		static string usage = "		Please provide a json file containing a 2D grid, \n" +
					"Example:\n\n" +
					"{\r\n  \"grid\": [\r\n    [ 1, 1, 0, 0 ],\r\n    [ 1, 1, 0, 0 ],\r\n    [ 0, 0, 1, 1 ],\r\n    [ 1, 1, 0, 1 ]\r\n  ]\r\n}";

		public static bool RunCLI(string file)
		{
			if (file.Length <= 0)
			{
				Console.WriteLine(usage);
				return false;
			}

			if (!ProcessGridFile(file))
			{
				Console.WriteLine(usage);
				return false;
			}

			return true;
		}

		public static bool ProcessGridFile(string file)
		{
			if (!File.Exists(file))
			{
				Console.WriteLine("The file does not exist.");
				return false;
			}

			Wrapper? jsongrid;

			try
			{
				using var stream = new FileStream(file, FileMode.Open);
				using var reader = new StreamReader(stream);
				string json = reader.ReadToEnd();

				jsongrid = JsonSerializer.Deserialize<Wrapper>(json);
			}
			catch (JsonException ex)
			{
				Console.WriteLine($"Error: file does not contain valid grid JSON. {ex.Message}");
				return false;
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error reading file: {ex.Message}");
				return false;
			}

			if (jsongrid?.grid == null)
			{
				Console.WriteLine("Error: JSON does not contain a 'grid' property. Or grid is null.");
				return false;
			}

			int[][] grid = jsongrid.grid;

			if (grid.Length == 0)
			{
				Console.WriteLine("The provided grid is empty. Number of Islands: 0");
				return true;
			}

			Graph<Node> graph = new Graph<Node>();
			graph.CreateGraphFromGrid(grid);

			IslandsBfs breadth = new IslandsBfs();
			var islands = breadth.CountIslands(graph);

			Console.WriteLine($"Number of Islands: {islands.Count}");

			if (islands.Count == 0)
			{
				Console.WriteLine("No Islands found.");
				return true;
			}

			var smallest = breadth.SmallestIsland;
			var largest = breadth.LargestIsland;

			Console.WriteLine($"Smallest Island size: {smallest.size}");

			foreach (var node in smallest.nodes)
			{
				Console.WriteLine($"	({node.Row}, {node.Column})");
			}

			Console.WriteLine($"Largest Island size: {largest.size}");

			foreach (var node in largest.nodes)
			{
				Console.WriteLine($"	({node.Row}, {node.Column})");
			}

			return true;
		}
	}
}