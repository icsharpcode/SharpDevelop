// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestResultsFileName
	{
		public MSTestResultsFileName(IEnumerable<ITest> selectedTests)
		{
			FileName = GetFileName(selectedTests);
		}
		
		public string FileName { get; private set; }
		
		string GetFileName(IEnumerable<ITest> selectedTests)
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Temp",
				selectedTests.First().ParentProject.Project.Name + "-Results.trx");
		}
	}
}
