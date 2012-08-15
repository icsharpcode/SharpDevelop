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
		
		public Globals(SD.Solution solution)
		{
			this.solution = solution;
		}
		
		public virtual bool VariableExists(string name)
		{
			SD.ProjectSection section = GetExtensibilityGlobalsSection();
			if (section != null) {
				return section.Items.Any(item => IsMatchIgnoringCase(item.Name, name));
			}
			return false;
		}
		
		bool IsMatchIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
		
		SD.ProjectSection GetExtensibilityGlobalsSection()
		{
			return solution.Sections.SingleOrDefault(section => section.Name == "ExtensibilityGlobals");
		}
	}
}
