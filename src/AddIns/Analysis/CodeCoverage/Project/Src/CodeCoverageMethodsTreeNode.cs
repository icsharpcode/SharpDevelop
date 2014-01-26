// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
				
		public CodeCoverageMethodsTreeNode(string name, List<CodeCoverageMethod> methods, CodeCoverageImageListIndex index) 
			: base(name, index)
		{
			this.methods = methods;
			AddDummyNodeIfHasNoMethods();
			
			int visitedCodeLength = 0;
			int unvisitedCodeLength = 0;
			decimal branchCoverage = 0;
			int branchCoverageCount = 0;
			foreach (CodeCoverageMethod method in methods) {
				if (method.Name.Contains("__")) {
					continue;
				}
				visitedCodeLength += method.GetVisitedCodeLength();
				unvisitedCodeLength += method.GetUnvisitedCodeLength();
				if ( method.IsVisited ) {
					branchCoverageCount += 1;
					branchCoverage += method.BranchCoverage == 0 ? 100 : method.BranchCoverage ;
				}
			}
			
			Name = name;
			VisitedCodeLength = visitedCodeLength;
			UnvisitedCodeLength = unvisitedCodeLength;
			VisitedBranchCoverage = branchCoverageCount == 0 ? 100 : branchCoverage/branchCoverageCount;
		}
		
		void AddDummyNodeIfHasNoMethods()
		{
			if (methods.Count > 0) {
				AddDummyNode();
			}
		}
		
		void AddDummyNode()
		{
			Nodes.Add(new ExtTreeNode());
		}
		
		public List<CodeCoverageMethod> Methods {
			get { return methods; }
		}
	}
}
