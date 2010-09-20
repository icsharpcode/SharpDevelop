// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// All Tests root tree node that is added to the test tree when the
	/// solution has multiple test projects.
	/// </summary>
	public class AllTestsTreeNode : TestTreeNode
	{
		public AllTestsTreeNode()
			: base(null, StringParser.Parse("${res:ICSharpCode.UnitTesting.AllTestsTreeNode.Text}"))
		{
		}
		
		/// <summary>
		/// Raised when the all tests tree node is disposed.
		/// </summary>
		public event EventHandler Disposed;
		
		/// <summary>
		/// Disposes this tree node.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
			if (Disposed != null) {
				Disposed(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// Adds a new project node as a child of the All Tests node.
		/// </summary>
		public void AddProjectNode(TestProjectTreeNode node)
		{
			node.AddTo(this);
			node.ImageIndexChanged += TestProjectTreeNodeImageIndexChanged;
		}
		
		/// <summary>
		/// Removes the project node.
		/// </summary>
		public void RemoveProjectNode(TestProjectTreeNode node)
		{
			if (Nodes.Contains(node)) {
				node.ImageIndexChanged -= TestProjectTreeNodeImageIndexChanged;
				node.Remove();
			}
		}
		
		void TestProjectTreeNodeImageIndexChanged(object source, EventArgs e)
		{
			UpdateImageListIndex();
		}
		
		/// <summary>
		/// Sets the All Tests image index based on the current image
		/// indexes of the child project tree nodes.
		/// </summary>
		void UpdateImageListIndex()
		{
			int ignored = 0;
			int failed = 0;
			int passed = 0;
			int notRun = 0;
			int total = Nodes.Count;
			
			foreach (TestProjectTreeNode projectNode in Nodes) {
				switch (projectNode.ImageIndex) {
					case (int)TestTreeViewImageListIndex.TestFailed:
						failed++;
						break;
					case (int)TestTreeViewImageListIndex.TestPassed:
						passed++;
						break;
					case (int)TestTreeViewImageListIndex.TestIgnored:
						ignored++;
						break;
					default: // Not run.
						notRun++;
						break;
				}
			}
				
			// Update the image index based on the test project results.
			if (failed > 0) {
				UpdateImageListIndex(TestResultType.Failure);
			} else if (ignored > 0) {
				UpdateImageListIndex(TestResultType.Ignored);
			} else if (passed > 0 && notRun == 0) {
				UpdateImageListIndex(TestResultType.Success);
			} else {
				UpdateImageListIndex(TestResultType.None);
			}
		}
	}
}
