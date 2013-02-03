// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageResultsReader
	{
		List<string> fileNames = new List<string>();
		IFileSystem fileSystem = new FileSystem();
		List<string> missingFileNames = new List<string>();
		
		public CodeCoverageResultsReader()
		{
		}
		
		public void AddResultsFile(string fileName)
		{
			fileNames.Add(fileName);
		}
		
		public IEnumerable<CodeCoverageResults> GetResults()
		{
			foreach (string fileName in fileNames) {
				if (fileSystem.FileExists(fileName)) {
					yield return ReadCodeCoverageResults(fileName);
				} else {
					missingFileNames.Add(fileName);
				}
			}
		}
		
		CodeCoverageResults ReadCodeCoverageResults(string fileName)
		{
			TextReader reader = fileSystem.CreateTextReader(fileName);
			return new CodeCoverageResults(reader);
		}
		
		public IEnumerable<string> GetMissingResultsFiles()
		{
			return missingFileNames;
		}
	}
}
