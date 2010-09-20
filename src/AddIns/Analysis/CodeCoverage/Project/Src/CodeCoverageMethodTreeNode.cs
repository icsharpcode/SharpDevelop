// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethodTreeNode : CodeCoverageTreeNode
	{
		CodeCoverageMethod method;
		
		public CodeCoverageMethodTreeNode(CodeCoverageMethod method)
			: base(method.Name, 
				CodeCoverageImageListIndex.Method,
				method.GetVisitedCodeLength(),
				method.GetUnvisitedCodeLength())
		{
			this.method = method;
		}
		
		public CodeCoverageMethod Method {
			get { return method; }
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
				
				JumpToFilePosition(firstSequencePoint.Document, line, column);
				
			} else if (Parent != null) {
				((ExtTreeNode)Parent).ActivateItem();
			}
		}
		
		/// <summary>
		/// Gets the string to be used when comparing method tree nodes
		/// on sorting.
		/// </summary>
		/// <remarks>
		/// Until we group the getter and setter into its own
		/// property tree node we return a compare string of the
		/// form "PropertyName set" so sorting is acceptable.
		/// </remarks>
		public override string CompareString {
			get { 
				if (IsGetterOrSetter(Name)) {
					string getterSetterPart = Name.Substring(0, 3);
					string property = Name.Substring(4);
					return String.Concat(property, ' ', getterSetterPart);
				}
				return base.CompareString;
			}
		}
		
		/// <summary>
		/// Determines if the name refers to a getter or setter method.
		/// </summary>
		static bool IsGetterOrSetter(string name)
		{
			return name.StartsWith("get_") || name.StartsWith("set_");
		}
	}
}
