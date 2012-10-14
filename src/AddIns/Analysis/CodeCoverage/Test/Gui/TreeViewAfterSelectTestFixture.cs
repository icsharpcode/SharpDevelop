// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
