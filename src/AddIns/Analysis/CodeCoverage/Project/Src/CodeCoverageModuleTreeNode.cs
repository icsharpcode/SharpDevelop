// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Represents an assembly in the code coverage tree view.
	/// </summary>
	public class CodeCoverageModuleTreeNode : CodeCoverageTreeNode
	{
		CodeCoverageModule module;
		
		public CodeCoverageModuleTreeNode(CodeCoverageModule module)
			: base(module, CodeCoverageImageListIndex.Module)
		{
			this.module = module;
			AddDummyNodeIfModuleHasNoMethods();
		}
		
		void AddDummyNodeIfModuleHasNoMethods()
		{
			if (module.Methods.Count > 0) {
				AddDummyNode();
			}
		}
		
		void AddDummyNode()
		{
			Nodes.Add(new ExtTreeNode());
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
			
			// Sort these nodes.
			SortChildNodes();
		}
	}
}
