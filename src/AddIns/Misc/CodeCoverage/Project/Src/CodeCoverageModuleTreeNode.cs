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
	public class CodeCoverageModuleTreeNode : CodeCoverageTreeNode
	{
		CodeCoverageModule module;
		
		public CodeCoverageModuleTreeNode(CodeCoverageModule module) : base(module.Name, CodeCoverageImageListIndex.Module, module.VisitedSequencePointsCount, module.NotVisitedSequencePointsCount)
		{
			this.module = module;
			if (module.Methods.Count > 0) {
				// Add dummy node.
				Nodes.Add(new ExtTreeNode());
			}
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			foreach (string namespaceName in module.RootNamespaces) {
				CodeCoverageNamespaceTreeNode node = new CodeCoverageNamespaceTreeNode(namespaceName, CodeCoverageMethod.GetAllMethods(module.Methods, namespaceName));
				node.AddTo(this);
			}
			
			// Add any classes that have no namespace.
			foreach (string className in CodeCoverageMethod.GetClassNames(module.Methods, String.Empty)) {
				CodeCoverageClassTreeNode classNode = new CodeCoverageClassTreeNode(className, CodeCoverageMethod.GetMethods(module.Methods, String.Empty, className));
				classNode.AddTo(this);
			}

		}
	}
}
