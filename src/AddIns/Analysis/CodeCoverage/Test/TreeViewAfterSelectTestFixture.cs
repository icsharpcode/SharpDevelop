// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
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
			DerivedExtTreeNode node = new DerivedExtTreeNode();
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
