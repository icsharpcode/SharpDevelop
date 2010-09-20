// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoveragePropertyTreeNode : CodeCoverageMethodsTreeNode
	{
		CodeCoverageProperty property;
		
		public CodeCoveragePropertyTreeNode(CodeCoverageProperty property) : 
			base(property.Name, property.GetMethods(), CodeCoverageImageListIndex.Property)
		{
			this.property = property;
		}
		
		public CodeCoverageProperty Property {
			get { return property; }
		}
		
		protected override void Initialize()
		{			
			Nodes.Clear();

			// Add methods.
			foreach (CodeCoverageMethod method in Methods) {
				CodeCoverageMethodTreeNode node = new CodeCoverageMethodTreeNode(method);
				node.AddTo(this);
			}
		}
	}
}
