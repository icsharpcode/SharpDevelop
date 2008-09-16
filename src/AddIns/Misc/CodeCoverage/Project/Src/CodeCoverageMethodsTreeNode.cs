// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Represents a code coverage tree node that has associated
	/// methods.
	/// </summary>
	public class CodeCoverageMethodsTreeNode : CodeCoverageTreeNode
	{
		List<CodeCoverageMethod> methods = new List<CodeCoverageMethod>();
				
		public CodeCoverageMethodsTreeNode(string name, List<CodeCoverageMethod> methods, CodeCoverageImageListIndex index) : base(name, index)
		{
			this.methods = methods;
			if (methods.Count > 0) {
				// Add dummy node.
				Nodes.Add(new ExtTreeNode());
			}
			
			int visitedCount = 0;
			int notVisitedCount = 0;
			foreach (CodeCoverageMethod method in methods) {
				visitedCount += method.VisitedSequencePointsCount;
				notVisitedCount += method.NotVisitedSequencePointsCount;
			}
			
			Name = name;
			VisitedCount = visitedCount;
			NotVisitedCount = notVisitedCount;
		}
		
		public List<CodeCoverageMethod> Methods {
			get { return methods; }
		}
	}
}
