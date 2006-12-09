// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageClassTreeNode : CodeCoverageMethodsTreeNode
	{
		public CodeCoverageClassTreeNode(string name, List<CodeCoverageMethod> methods) : base(name, methods, CodeCoverageImageListIndex.Class)
		{
		}

		public override void ActivateItem()
		{
			if (Nodes.Count > 0) {
				CodeCoverageMethodTreeNode methodNode = (CodeCoverageMethodTreeNode)Nodes[0];
				if (methodNode.Method.SequencePoints.Count > 0) {
					FileService.OpenFile(methodNode.Method.SequencePoints[0].Document);
				}
			}
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			foreach (CodeCoverageMethod method in Methods) {
				CodeCoverageMethodTreeNode node = new CodeCoverageMethodTreeNode(method);
				node.AddTo(this);
			}
			
			// Sort nodes.
			SortChildNodes();
		}
	}
}
