// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class SolutionTreeNode : ModelCollectionTreeNode
	{
		ISolution solution;
		
		public SolutionTreeNode(ISolutionAssemblyList solutionAssemblyList)
		{
			if (solutionAssemblyList == null)
				throw new ArgumentNullException("solutionAssemblyList");
			this.solution = solutionAssemblyList.Solution;
		}
		
		protected override object GetModel()
		{
			return solution;
		}
		
		public override object Text {
			get {
				return "Solution " + solution.Name;
			}
		}
		
		public override object Icon {
			get {
				return IconService.GetImageSource("Icons.16x16.SolutionIcon");
			}
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get {
				return NodeTextComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return solution.Projects;
			}
		}
	}
}


