// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;

using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestApplication
	{
		SelectedTests selectedTests;
		string resultsFileName;
		
		public MSTestApplication(SelectedTests selectedTests, string resultsFileName)
		{
			this.selectedTests = selectedTests;
			this.resultsFileName = resultsFileName;
			GetProcessStartInfo();
		}
		
		void GetProcessStartInfo()
		{
			ProcessStartInfo = new ProcessStartInfo(MSTestOptions.MSTestPath, GetCommandLine());
		}
		
		string GetCommandLine()
		{
			var commandLine = new MSTestApplicationCommandLine();
			commandLine.AppendQuoted("testcontainer", selectedTests.Project.OutputAssemblyFullPath);
			commandLine.AppendQuoted("resultsfile", resultsFileName);
			commandLine.Append("detail", "errorstacktrace");
			if (selectedTests.NamespaceFilter != null) {
				commandLine.Append("test", selectedTests.NamespaceFilter);
			} else if (selectedTests.Member != null) {
				commandLine.Append("test", selectedTests.Member.FullyQualifiedName);
			} else if (selectedTests.Class != null) {
				commandLine.Append("test", selectedTests.Class.FullyQualifiedName);
			}
			return commandLine.ToString();
		}
		
		public ProcessStartInfo ProcessStartInfo { get; private set; }
	}
}
