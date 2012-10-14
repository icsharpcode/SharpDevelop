// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.CodeCoverage;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	/// <summary>
	/// Derived code coverage tree view that gives us access to the
	/// AfterSelect method.
	/// </summary>
	public class DerivedCodeCoverageTreeView : CodeCoverageTreeView
	{
		public void CallOnAfterSelect(TreeNode node)
		{
			base.OnAfterSelect(new TreeViewEventArgs(node));
		}
	}
	
	/// <summary>
	/// Derived CodeCoverageTreeNode class so we can check the IsInitialized
	/// property.
	/// </summary>
	public class DerivedCodeCoverageTreeNode : CodeCoverageTreeNode
	{
		public DerivedCodeCoverageTreeNode(string name, CodeCoverageImageListIndex index)
			: base(name, index)
		{
		}
	}
}
