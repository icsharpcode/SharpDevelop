// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.CodeCoverage;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage.Tests
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
	/// Derived ExtTreeNode class so we can check the IsInitialized
	/// property.
	/// </summary>
	public class DerivedExtTreeNode : ExtTreeNode
	{
		public bool IsInitialized {
			get {
				return base.isInitialized;
			}
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
		
		public bool IsInitialized {
			get {
				return base.isInitialized;
			}
		}
	}
}
