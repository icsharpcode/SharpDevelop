// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestResultsFileName
	{
		public MSTestResultsFileName(SelectedTests selectedTests)
		{
			FileName = GetFileName(selectedTests);
		}
		
		public string FileName { get; private set; }
		
		string GetFileName(SelectedTests selectedTests)
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Temp",
				selectedTests.Project.Name + "-Results.trx");
		}
	}
}
