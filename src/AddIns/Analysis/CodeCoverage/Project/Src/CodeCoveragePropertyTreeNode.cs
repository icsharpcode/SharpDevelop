// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoveragePropertyTreeNode : CodeCoverageMethodsTreeNode
	{
		CodeCoverageProperty property;
		
		public CodeCoveragePropertyTreeNode(CodeCoverageProperty property) : base(property.Name, property.GetMethods(), CodeCoverageImageListIndex.Property)
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
