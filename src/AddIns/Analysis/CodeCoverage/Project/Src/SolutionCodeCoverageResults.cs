// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	public class SolutionCodeCoverageResults
	{
		Solution solution;
		IFileSystem fileSystem;
		
		public SolutionCodeCoverageResults(Solution solution)
			: this(solution, new FileSystem())
		{
		}
		
		public SolutionCodeCoverageResults(Solution solution, IFileSystem fileSystem)
		{
			this.solution = solution;
			this.fileSystem = fileSystem;
		}
		
		public IEnumerable<CodeCoverageResults> GetCodeCoverageResultsForAllProjects()
		{
			foreach (IProject project in solution.Projects) {
				CodeCoverageResults results = GetCodeCoverageResultsForProject(project);
				if (results != null) {
					yield return results;
				}
			}
		}
		
		CodeCoverageResults GetCodeCoverageResultsForProject(IProject project)
		{
			var fileName = new ProjectCodeCoverageResultsFileName(project);
			if (fileSystem.FileExists(fileName.FileName)) {
				TextReader reader = fileSystem.CreateTextReader(fileName.FileName);
				return new CodeCoverageResults(reader);
			}
			return null;
		}
	}
}
