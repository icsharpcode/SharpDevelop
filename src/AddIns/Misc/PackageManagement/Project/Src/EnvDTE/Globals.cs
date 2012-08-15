// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Globals
	{
		SD.Solution solution;
		SolutionExtensibilityGlobals extensibilityGlobals;
		
		public Globals(SD.Solution solution)
		{
			this.solution = solution;
			this.extensibilityGlobals = new SolutionExtensibilityGlobals(solution);
		}
		
		public virtual SolutionExtensibilityGlobals VariableValue {
			get { return extensibilityGlobals; }
		}
		
		public virtual bool VariableExists(string name)
		{
			return extensibilityGlobals.GetItemFromSolution(name) != null;
		}
	}
}
