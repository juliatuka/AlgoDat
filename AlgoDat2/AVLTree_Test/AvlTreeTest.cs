using AlgoDat2;

namespace AVLTree_Test
{
	[TestFixture]
	public class AvlTreeTest
	{
		private AvlTree<int> _tree;

		[SetUp]
		public void Setup()
		{
			_tree = new AvlTree<int>();
		}

		[Test]
		public void Initial_Tree_Is_Empty()
		{
			_tree = new AvlTree<int> ();
			Assert.That(_tree, Is.Not.Null);
			Assert.That(_tree.Root, Is.Null);
		}

		[Test]
		public void Add_Single_Element_BecomesRoot()
		{
			_tree.Add(12);
			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root?.Value, Is.EqualTo(12));
			Assert.That(_tree.Root?.LeftChild, Is.Null);
			Assert.That(_tree.Root?.RightChild, Is.Null);
		}

		[Test]
		public void Adding_Single_Element_Has_Height_Of_One()
		{
			_tree.Add(12);
			Assert.That(_tree.Root.Height, Is.EqualTo(1));
		}

		[Test]
		public void Adding_A_Lower_Value_Becomes_LeftChild()
		{
			_tree.Add(12);
			_tree.Add(9);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root?.Value, Is.EqualTo(12));
			Assert.That(_tree?.Root?.Height, Is.EqualTo(2));
			Assert.That(_tree?.Root?.LeftChild, Is.Not.Null);
			Assert.That(_tree?.Root?.LeftChild?.Value, Is.EqualTo(9));
			Assert.That(_tree?.Root?.LeftChild?.Height, Is.EqualTo(1));
			Assert.That(_tree?.Root?.RightChild, Is.Null);
			Assert.That(_tree?.Root?.LeftChild?.LeftChild, Is.Null);
			Assert.That(_tree?.Root?.LeftChild?.RightChild, Is.Null);
		}

		[Test]
		public void Adding_A_Higher_Value_Becomes_RightChild()
		{
			_tree.Add(9);
			_tree.Add(12);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root?.Value, Is.EqualTo(9));
			Assert.That(_tree?.Root?.Height, Is.EqualTo(2));
			Assert.That(_tree?.Root?.RightChild, Is.Not.Null);
			Assert.That(_tree?.Root?.RightChild?.Value, Is.EqualTo(12));
			Assert.That(_tree?.Root?.RightChild?.Height, Is.EqualTo(1));
			Assert.That(_tree?.Root?.LeftChild, Is.Null);
			Assert.That(_tree?.Root?.RightChild?.RightChild, Is.Null);
			Assert.That(_tree?.Root?.RightChild?.LeftChild, Is.Null);
		}

		[Test]
		public void Add_Multiple_Elements_All_Searchable()
		{
			int[] items = { 11, 2, 13, 3, 7, 8, 20 };

			foreach (int item in items)
				_tree.Add(item);

			foreach (var item in items)
			{
				Assert.IsTrue(_tree.Search(item));
			}

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root?.Value, Is.EqualTo(7));
			Assert.That(_tree.Root?.Height, Is.EqualTo(4));
			Assert.That(_tree.Root?.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root?.LeftChild?.Value, Is.EqualTo(3));
			Assert.That(_tree.Root?.LeftChild?.Height, Is.EqualTo(2));
			Assert.That(_tree.Root?.LeftChild?.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root?.LeftChild?.LeftChild?.Value, Is.EqualTo(2));
			Assert.That(_tree.Root?.LeftChild?.LeftChild?.Height, Is.EqualTo(1));
			Assert.That(_tree.Root?.RightChild, Is.Not.Null);
			Assert.That(_tree.Root?.RightChild?.Value, Is.EqualTo(11));
			Assert.That(_tree.Root?.RightChild?.Height, Is.EqualTo(3));
			Assert.That(_tree.Root?.RightChild?.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root?.RightChild?.LeftChild?.Value, Is.EqualTo(8));
			Assert.That(_tree.Root?.RightChild?.LeftChild?.Height, Is.EqualTo(1));
			Assert.That(_tree.Root?.RightChild?.RightChild, Is.Not.Null);
			Assert.That(_tree.Root?.RightChild?.RightChild?.Value, Is.EqualTo(13));
			Assert.That(_tree.Root?.RightChild?.RightChild?.Height, Is.EqualTo(2));
			Assert.That(_tree.Root?.RightChild?.RightChild?.RightChild, Is.Not.Null);
			Assert.That(_tree.Root?.RightChild?.RightChild?.RightChild?.Value, Is.EqualTo(20));
			Assert.That(_tree.Root?.RightChild?.RightChild?.RightChild?.Height, Is.EqualTo(1));

			Assert.That(_tree.Root?.LeftChild?.RightChild, Is.Null);
			Assert.That(_tree.Root?.LeftChild?.LeftChild?.LeftChild, Is.Null);
			Assert.That(_tree.Root?.LeftChild?.LeftChild?.RightChild, Is.Null);
			Assert.That(_tree.Root?.RightChild?.RightChild?.LeftChild, Is.Null);
			Assert.That(_tree.Root?.RightChild?.RightChild?.RightChild?.LeftChild, Is.Null);
			Assert.That(_tree.Root?.RightChild?.RightChild?.RightChild?.RightChild, Is.Null);
		}

		[Test]
		public void Adding_Duplicates_Ignored()
		{
			_tree.Add(12);
			_tree.Add(12);

			Assert.IsTrue( _tree.Search(12));

			Assert.That(_tree.Root.Value, Is.EqualTo(12));
			Assert.That(_tree.Root.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild, Is.Null);
		}

		[Test]
		public void Adding_ELements_Only_Left_Triggers_LLRotation_Middle_Becomes_Root()
		{
			_tree.Add(3);
			_tree.Add(2);
			_tree.Add(1);
			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(3));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Adding_ELements_Only_Right_Triggers_RRRotation_Middle_Becomes_Root()
		{
			_tree.Add(1);
			_tree.Add(2);
			_tree.Add(3);
			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(3));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Adding_Elements_Smaller_Than_Larger_Triggers_LRRotation_Middle_Becomes_Root()
		{
			_tree.Add(3);
			_tree.Add(1);
			_tree.Add(2);
			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(3));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Adding_Elements_Larger_Than_Smaller_Triggers_RLRotation_Middle_Becomes_Root()
		{
			_tree.Add(1);
			_tree.Add(3);
			_tree.Add(2);
			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(3));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Add_NullElement_Throws_ArgumentNullException()
		{
			AvlTree<string> tree = new AvlTree<string>();

			Assert.Throws<ArgumentNullException>(() => tree.Add(null));
		}

		[Test]
		public void Remove_NullElement_Throws_ArgumentNullException()
		{
			AvlTree<string> tree = new AvlTree<string>();

			Assert.Throws<ArgumentNullException>(() => tree.Remove(null));
		}

		[Test]
		public void Remove_LeafNode_Not_Searchable()
		{
			_tree.Add(10);
			_tree.Add(4);
			_tree.Add(6);
			_tree.Add(2);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.LeftChild.Height, Is.EqualTo(1));

			_tree.Remove(2);
			Assert.IsFalse(_tree.Search(2));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_Node_With_One_Child_Not_Searchable()
		{
			_tree.Add(10);
			_tree.Add(4);
			_tree.Add(6);
			_tree.Add(2);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.LeftChild.Height, Is.EqualTo(1));

			_tree.Remove(4);

			Assert.IsFalse(_tree.Search(4));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_Node_With_Two_Children_Tree_Stays_Balanced() 
		{
			_tree.Add(10);
			_tree.Add(4);
			_tree.Add(6);
			_tree.Add(2);
			_tree.Add(3);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.RightChild.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.LeftChild.RightChild.Height, Is.EqualTo(1));

			_tree.Remove(3);

			Assert.IsFalse(_tree.Search(3));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_RootNode_Root_Deleted_Tree_Stays_Balanced() 
		{
			_tree.Add(10);
			_tree.Add(4);
			_tree.Add(6);
			_tree.Add(2);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(6));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.LeftChild.Height, Is.EqualTo(1));

			_tree.Remove(6);

			Assert.IsFalse(_tree.Search(6));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(4));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_Element_Triggers_LLRotation_Middle_Value_Becomes_Root()
		{
			_tree.Add(13);
			_tree.Add(10);
			_tree.Add(16);
			_tree.Add(5);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(16));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.LeftChild.Value, Is.EqualTo(5));
			Assert.That(_tree.Root.LeftChild.LeftChild.Height, Is.EqualTo(1));

			_tree.Remove(16);

			Assert.IsFalse(_tree.Search(16));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(5));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_Element_Triggers_RRRotation_Middle_Value_Becomes_Root()
		{
			_tree.Add(10);
			_tree.Add(13);
			_tree.Add(5);
			_tree.Add(16);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(5));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.RightChild.Value, Is.EqualTo(16));
			Assert.That(_tree.Root.RightChild.RightChild.Height, Is.EqualTo(1));

			_tree.Remove(5);

			Assert.IsFalse(_tree.Search(5));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(16));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_Element_Triggers_LRRotation_Middle_Value_Becomes_Root()
		{
			_tree.Add(13);
			_tree.Add(10);
			_tree.Add(16);
			_tree.Add(11);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(16));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.RightChild.Value, Is.EqualTo(11));
			Assert.That(_tree.Root.LeftChild.RightChild.Height, Is.EqualTo(1));

			_tree.Remove(16);

			Assert.IsFalse(_tree.Search(16));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(11));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(10));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_Element_Triggers_RLRotation_Middle_Value_Becomes_Root()
		{
			_tree.Add(16);
			_tree.Add(13);
			_tree.Add(35);
			_tree.Add(20);

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(16));
			Assert.That(_tree.Root.Height, Is.EqualTo(3));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(13));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(35));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.LeftChild.Value, Is.EqualTo(20));
			Assert.That(_tree.Root.RightChild.LeftChild.Height, Is.EqualTo(1));

			_tree.Remove(13);

			Assert.IsFalse(_tree.Search(13));

			Assert.That(_tree.Root, Is.Not.Null);
			Assert.That(_tree.Root.Value, Is.EqualTo(20));
			Assert.That(_tree.Root.Height, Is.EqualTo(2));
			Assert.That(_tree.Root.LeftChild, Is.Not.Null);
			Assert.That(_tree.Root.LeftChild.Value, Is.EqualTo(16));
			Assert.That(_tree.Root.LeftChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.RightChild, Is.Not.Null);
			Assert.That(_tree.Root.RightChild.Value, Is.EqualTo(35));
			Assert.That(_tree.Root.RightChild.Height, Is.EqualTo(1));
			Assert.That(_tree.Root.LeftChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.LeftChild.RightChild, Is.Null);
			Assert.That(_tree.Root.RightChild.LeftChild, Is.Null);
			Assert.That(_tree.Root.RightChild.RightChild, Is.Null);
		}

		[Test]
		public void Remove_NonExisting_DoesNotThrow_Exception()
		{
			_tree.Add(10);
			_tree.Add(4);
			_tree.Add(6);
			_tree.Add(2);

			Assert.DoesNotThrow(() => _tree.Remove(1));

			Assert.IsTrue(_tree.Search(6));
			Assert.IsTrue(_tree.Search(2));
			Assert.IsTrue(_tree.Search(4));
			Assert.IsTrue(_tree.Search(10));
		}

		[Test]
		public void Remove_From_Empty_DoesNotThrow_Exception()
		{
			Assert.DoesNotThrow(() => _tree.Remove(2));
		}

		[Test]
		public void Search_OnEmpty_Returns_False()
		{
			Assert.IsFalse(_tree.Search(10));
		}

		[Test]
		public void Search_For_Existing_Element_Returns_True()
		{
			_tree.Add(2);
			_tree.Add(4);
			_tree.Add(10);
			_tree.Add(6);

			Assert.IsTrue(_tree.Search(2));
		}

		[Test]
		public void Search_For_Nonexisting_Element_Returns_False()
		{
			_tree.Add(2);
			_tree.Add(4);
			_tree.Add(10);
			_tree.Add(6);

			Assert.IsFalse(_tree.Search(12));
		}
	}
}
