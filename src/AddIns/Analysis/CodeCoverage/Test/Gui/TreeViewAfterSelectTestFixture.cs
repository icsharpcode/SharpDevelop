// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.CodeCoverage.Tests.Utils;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Gui
{
	/// <summary>
	/// Tests the CodeCoverageResultsTreeView AfterSelect method.
	/// </summary>
	[TestFixture]
	public class TreeViewAfterSelectTestFixture
	{
		/// <summary>
		/// Code coverage tree node should be initialised in OnAfterSelect method.
		/// </summary>
		[Test]
		public void NodeInitialised()
		{
			DerivedCodeCoverageTreeView treeView = new DerivedCodeCoverageTreeView();
			DerivedCodeCoverageTreeNode node = new DerivedCodeCoverageTreeNode("Test", CodeCoverageImageListIndex.Class);
			treeView.CallOnAfterSelect(node);
			Assert.IsTrue(node.IsInitialized);
		}
		
		/// <summary>
		/// Non code coverage tree node should not initialised in OnAfterSelect method.
		/// </summary>
		[Test]
		public void NonCodeCoverageNodeNotInitialised()
		{
			DerivedCodeCoverageTreeView treeView = new DerivedCodeCoverageTreeView();
			ExtTreeNode node = new ExtTreeNode();
			treeView.CallOnAfterSelect(node);
			Assert.IsFalse(node.IsInitialized);
		}
		
		/// <summary>
		/// Check that the OnAfterSelect method handles a null node.
		/// </summary>
		[Test]
		public void NullNode()
		{
			DerivedCodeCoverageTreeView treeView = new DerivedCodeCoverageTreeView();
			treeView.CallOnAfterSelect(null);
		}
	}
}
