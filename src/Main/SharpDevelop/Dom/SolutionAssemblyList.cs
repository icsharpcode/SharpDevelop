// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Implements an assembly list representing a solution.
	/// </summary>
	class SolutionAssemblyList : ISolutionAssemblyList
	{
		ISolution solution;

		public SolutionAssemblyList(ISolution solution)
		{
			this.solution = solution;
			if (solution != null) {
				Name = solution.Name;
				Assemblies = new NullSafeSimpleModelCollection<IAssemblyModel>();
				Assemblies.AddRange(solution.Projects.Select(p => p.AssemblyModel));
			}
		}
		
		public ISolution Solution {
			get {
				return solution;
			}
		}
		
		public string Name { get; set; }

		public IMutableModelCollection<IAssemblyModel> Assemblies { get; set; }
	}
}