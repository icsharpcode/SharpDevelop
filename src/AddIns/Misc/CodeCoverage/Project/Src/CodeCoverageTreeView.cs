// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageTreeView : ExtTreeView
	{
		public CodeCoverageTreeView()
		{
		}
		
		public void AddModules(List<CodeCoverageModule> modules)
		{
			foreach (CodeCoverageModule module in modules) {
				Nodes.Add(new CodeCoverageModuleTreeNode(module));
			}
		}
	}
}
