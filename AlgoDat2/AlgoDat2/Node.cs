using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoDat2
{
	public class Node<T> where T : IComparable<T>
	{
		private Node<T>? _leftChild;
		private Node<T>? _rightChild;
		private T _value;
		private int _height;

		public Node(T data)
		{
			Value = data;
			Height = 1; // 1
		}

		public Node<T>? LeftChild
		{
			get { return _leftChild; }
			set { _leftChild = value; }
		}

		public Node<T>? RightChild
		{
			get { return _rightChild; }
			set { _rightChild = value; }
		}

		public T Value
		{
			get { return _value; }	
			set { _value = value; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}
	}
}
