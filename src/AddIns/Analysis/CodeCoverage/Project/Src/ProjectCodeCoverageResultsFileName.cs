// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	public class ProjectCodeCoverageResultsFileName
	{
		public ProjectCodeCoverageResultsFileName(IProject project)
		{
			FileName = GetCodeCoverageResultsFileName(project);
		}
		
		public string FileName { get; private set; }
		
		string GetCodeCoverageResultsFileName(IProject project)
		{
			string  outputDirectory = GetOutputDirectory(project);
			return Path.Combine(outputDirectory, "coverage.xml");
		}
		
		string GetOutputDirectory(IProject project)
		{
			return Path.Combine(project.Directory, "OpenCover");
		}
	}
}
