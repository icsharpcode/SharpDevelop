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
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageNamespaceTreeNode : CodeCoverageMethodsTreeNode
	{
		string namespacePrefix = String.Empty;
				
		public CodeCoverageNamespaceTreeNode(string name, List<CodeCoverageMethod> methods)
			: this(String.Empty, name, methods)
		{
		}
		
		public CodeCoverageNamespaceTreeNode(string namespacePrefix, string name, List<CodeCoverageMethod> methods) : base(name, methods, CodeCoverageImageListIndex.Namespace)
		{
			sortOrder = 1;
			this.namespacePrefix = namespacePrefix;
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			
			// Add namespace nodes.
			string fullNamespace = CodeCoverageMethod.GetFullNamespace(namespacePrefix, Name);
			foreach (string namespaceName in CodeCoverageMethod.GetChildNamespaces(Methods, fullNamespace)) {
				string childFullNamespace = CodeCoverageMethod.GetFullNamespace(fullNamespace, namespaceName);
				CodeCoverageNamespaceTreeNode node = new CodeCoverageNamespaceTreeNode(fullNamespace, namespaceName, CodeCoverageMethod.GetAllMethods(Methods, childFullNamespace));
				node.AddTo(this);
			}
			
			// Add class nodes for this namespace.
			foreach (string className in CodeCoverageMethod.GetClassNames(Methods, fullNamespace)) {
				CodeCoverageClassTreeNode classNode = new CodeCoverageClassTreeNode(className, CodeCoverageMethod.GetMethods(Methods, fullNamespace, className));
				classNode.AddTo(this);
			}
			
			// Sort nodes added.
			SortChildNodes();
		}
	}
}
