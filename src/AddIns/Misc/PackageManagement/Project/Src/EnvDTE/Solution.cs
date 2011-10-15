// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Solution : MarshalByRefObject
	{
		SD.Solution solution;
		
		public Solution(SD.Solution solution)
		{
			this.solution = solution;
		}
		
		public string FullName {
			get { return FileName; }
		}
		
		public string FileName {
			get { return solution.FileName; }
		}
	}
}
