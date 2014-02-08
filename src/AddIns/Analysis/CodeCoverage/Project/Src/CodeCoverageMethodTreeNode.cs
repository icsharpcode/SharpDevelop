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
				method.GetUnvisitedCodeLength(),
				method.BranchCoverage
			)
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
