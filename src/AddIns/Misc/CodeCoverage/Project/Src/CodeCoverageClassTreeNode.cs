// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageClassTreeNode : CodeCoverageTreeNode
	{
		public CodeCoverageClassTreeNode(string name) : base(name)
		{
			ImageIndex = ClassBrowserIconService.ClassIndex;
			SelectedImageIndex = ImageIndex;
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
		
	}
}
