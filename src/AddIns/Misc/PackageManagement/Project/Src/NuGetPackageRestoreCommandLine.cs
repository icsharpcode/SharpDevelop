// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class NuGetPackageRestoreCommandLine
	{
		List<string> arguments = new List<string>();
		
		public NuGetPackageRestoreCommandLine(IPackageManagementSolution solution)
		{
			GenerateCommandLine(solution);
		}
		
		public string Command { get; set; }
		
		public string[] Arguments {
			get { return arguments.ToArray(); }
		}
		
		void GenerateCommandLine(IPackageManagementSolution solution)
		{
			arguments.Add("restore");
			arguments.Add(solution.FileName);
		}
	}
}
