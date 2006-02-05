// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethodTreeNode : CodeCoverageTreeNode
	{
		CodeCoverageMethod method;
		
		public CodeCoverageMethodTreeNode(CodeCoverageMethod method) : base(method.Name, CodeCoverageImageListIndex.Method, method.VisitedSequencePointsCount, method.NotVisitedSequencePointsCount)
		{
			this.method = method;
		}
		
		public CodeCoverageMethod Method {
			get {
				return method;
			}
		}
		
		public override void ActivateItem()
		{
			if (method != null && method.SequencePoints.Count > 0) {
				CodeCoverageSequencePoint firstSequencePoint = method.SequencePoints[0];
				int line = firstSequencePoint.Line;
				int column = firstSequencePoint.Column;
				
				for (int i = 1; i < method.SequencePoints.Count; ++i) {
					CodeCoverageSequencePoint sequencePoint = method.SequencePoints[0];
					if (line > sequencePoint.Line) {
						line = sequencePoint.Line;
						column = sequencePoint.Column;
					}
				}
				
				FileService.JumpToFilePosition(firstSequencePoint.Document, line - 1, column - 1);
				
			} else if (Parent != null) {
				((ExtTreeNode)Parent).ActivateItem();
			}
		}
		
	}
}
