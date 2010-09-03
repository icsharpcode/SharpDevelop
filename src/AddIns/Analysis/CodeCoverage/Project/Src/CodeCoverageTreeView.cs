// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageTreeView : ExtTreeView
	{
		public CodeCoverageTreeView()
		{
		}
		
		public void AddModules(List<CodeCoverageModule> modules)
		{
			BeginUpdate();
			try {
				foreach (CodeCoverageModule module in modules) {
					Nodes.Add(new CodeCoverageModuleTreeNode(module));
				}
				Sort();
			} finally {
				EndUpdate();
			}
		}
		
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			CodeCoverageTreeNode node = e.Node as CodeCoverageTreeNode;
			if (node != null) {
				node.PerformInitialization();
			}
			base.OnAfterSelect(e);
		}
	}
}
