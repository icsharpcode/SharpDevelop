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
