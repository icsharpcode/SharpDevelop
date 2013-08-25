// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class NuGetPackageRestoreCommandLine
	{
		public NuGetPackageRestoreCommandLine(IPackageManagementSolution solution)
		{
			GenerateCommandLine(solution);
		}
		
		public string Command { get; set; }
		public string Arguments { get; private set; }
		
		void GenerateCommandLine(IPackageManagementSolution solution)
		{
			Arguments = String.Format("restore \"{0}\"", solution.FileName);
		}
		
		public override string ToString()
		{
			return String.Format("{0} {1}", GetQuotedCommand(), Arguments);
		}
		
		string GetQuotedCommand()
		{
			return String.Format("\"{0}\"", Command);
		}
	}
}
