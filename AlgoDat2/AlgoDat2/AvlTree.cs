using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AlgoDat2
{
	public class AvlTree<T> where T : IComparable<T>
	{
		private Node<T> _root;

		public Node<T> Root
		{
			get { return _root; }
			private set { _root = value; }
		}

		public void Add(T item)
		{
			if (item == null) throw new ArgumentNullException("The value of the item to be added can not be null");

			if (Root == null)
			{
				Root = new Node<T>(item);
				return;
			}

			Root = Add(item, Root);
		}

		private Node<T> Add(T item, Node<T> current)
		{
			if (current == null)
			{
				return new Node<T>(item);
			}

			int compared = item.CompareTo(current.Value);

			if (compared == 0)
			{
				return current;
			}
			else if (compared < 0) // item < current 
			{
				current.LeftChild = Add(item, current.LeftChild);
			}
			else // item > current
			{
				current.RightChild = Add(item, current.RightChild);
			}

			current.Height = GetCurrentHeight(current);

			int balanceFactor = GetBalanceFactor(current);

			// LL - left-heavy
			if (balanceFactor > 1 && item.CompareTo(current.LeftChild.Value) < 0)
			{
				return RotateRight(current);
			}

			//LR - left-heavy
			if (balanceFactor > 1 && item.CompareTo(current.LeftChild.Value) > 0)
			{
				current.LeftChild = RotateLeft(current.LeftChild);
				return RotateRight(current);
			}

			// RR - right-heavy
			if (balanceFactor < -1 && item.CompareTo(current.RightChild.Value) > 0)
			{
				return RotateLeft(current);
			}

			//RL - right-heavy
			if (balanceFactor < -1 && item.CompareTo(current.RightChild.Value) < 0)
			{
				current.RightChild = RotateRight(current.RightChild);
				return RotateLeft(current);
			}

			return current;
		}

		private Node<T> RotateLeft(Node<T> current)
		{
			Node<T> newSubRoot = current.RightChild;
			Node<T> currentLeft = newSubRoot.LeftChild;

			// Perform rotation
			newSubRoot.LeftChild = current;
			current.RightChild = currentLeft;

			// Update heights
			current.Height = GetCurrentHeight(current);
			newSubRoot.Height = GetCurrentHeight(newSubRoot);

			// Return new root
			return newSubRoot;
		}

		private Node<T> RotateRight(Node<T> current)
		{
			Node<T> newSubRoot = current.LeftChild;
			Node<T> currentRight = newSubRoot.RightChild;

			// Perform rotation
			newSubRoot.RightChild = current;
			current.LeftChild = currentRight;

			// Update heights
			current.Height = GetCurrentHeight(current);
			newSubRoot.Height = GetCurrentHeight(newSubRoot);

			// Return new root
			return newSubRoot;
		}

		private int GetCurrentHeight(Node<T> current)
		{
			if (current == null) return 0;

			return 1 + Math.Max(current.LeftChild != null ? current.LeftChild.Height : 0, // ? 0
				current.RightChild != null ? current.RightChild.Height : 0); // ? 0
		}

		private int GetBalanceFactor(Node<T> current)
		{
			if (current == null)
				return 0;

			return (current.LeftChild != null ? current.LeftChild.Height : 0) - (current.RightChild != null ? current.RightChild.Height : 0);
		}

		public void Remove(T item)
		{
			if (item == null) throw new ArgumentNullException("The value of the item to be removed can not be null");

			if (Root == null)
			{
				return;
			}

			Root = Remove(item, Root);
		}

		private Node<T> Remove(T item, Node<T> current)
		{
			if (current == null)
			{
				return current; // if item does not exist
			}

			int compared = item.CompareTo(current.Value);

			if (compared < 0)
			{
				current.LeftChild = Remove(item, current.LeftChild);
			}
			else if (compared > 0)
			{
				current.RightChild = Remove(item, current.RightChild);
			}
			else //if (compared == 0) // item found
			{
				if (current.LeftChild == null || current.RightChild == null)
				{
					Node<T> temp = current.LeftChild != null ? current.LeftChild : current.RightChild;

					if (temp == null)
					{
						current = null;
					}
					else
					{
						current = temp;
					}
				}
				else
				{
					Node<T> temp = GetSmallestNode(current.RightChild);

					current.Value = temp.Value;

					current.RightChild = Remove(temp.Value, current.RightChild);
				}
			}

			if (current == null)
			{
				return current;
			}

			current.Height = GetCurrentHeight(current);

			int balanceFactor = GetBalanceFactor(current);

			// LL - left-heavy
			if (balanceFactor > 1 && GetBalanceFactor(current.LeftChild) >= 0)
			{
				return RotateRight(current);
			}

			//LR - left-heavy
			if (balanceFactor > 1 && GetBalanceFactor(current.LeftChild) < 0)
			{
				current.LeftChild = RotateLeft(current.LeftChild);
				return RotateRight(current);
			}

			// RR - right-heavy
			if (balanceFactor < -1 && GetBalanceFactor(current.RightChild) <= 0)
			{
				return RotateLeft(current);
			}

			//RL - right-heavy
			if (balanceFactor < -1 && GetBalanceFactor(current.RightChild) > 0)
			{
				current.RightChild = RotateRight(current.RightChild);
				return RotateLeft(current);
			}

			return current;
		}

		private Node<T> GetSmallestNode(Node<T> node)
		{
			Node<T> current = node;

			while (current.LeftChild != null)
				current = current.LeftChild;


			return current;
		}

		//public void PrintTree()
		//{
		//	PrintTree(Root, "", true);
		//}

		//private void PrintTree(Node<T> node, string prefix, bool isLeft)
		//{
		//	if (node == null)
		//	{
		//		return;
		//	}

		//	Console.WriteLine(prefix + (isLeft ? "├── " : "└── ") + node.Value);

		//	if (node.LeftChild != null || node.RightChild != null)
		//	{
		//		PrintTree(node.LeftChild, prefix + (isLeft ? "│   " : "    "), true);
		//		PrintTree(node.RightChild, prefix + (isLeft ? "│   " : "    "), false);
		//	}
		//}

		public bool Search(T item)
		{
			if (Root == null)
			{
				return false;
			}

			return Search(item, Root);
		}

		public bool Search(T item, Node<T> current)
		{
			if (current == null)
			{
				return false;
			}

			if (item.CompareTo(current.Value) == 0)
			{
				return true;
			}

			if (item.CompareTo(current.Value) > 0)
			{
				return Search(item, current.RightChild);
			}
			else
			{
				return Search(item, current.LeftChild);
			}
		}
	}
}
