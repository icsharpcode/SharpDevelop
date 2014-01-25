// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestApplication
	{
		readonly IEnumerable<ITest> selectedTests;
		readonly string resultsFileName;
		
		public MSTestApplication(IEnumerable<ITest> selectedTests, string resultsFileName)
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
			ITest test = selectedTests.FirstOrDefault();
			
			var commandLine = new MSTestApplicationCommandLine();
			commandLine.AppendQuoted("testcontainer", test.ParentProject.Project.OutputAssemblyFullPath);
			commandLine.AppendQuoted("resultsfile", resultsFileName);
			commandLine.Append("detail", "errorstacktrace");
			if (test is TestNamespace) {
				commandLine.Append("test", ((TestNamespace)test).NamespaceName);
			} else if (test is MSTestMember) {
				commandLine.Append("test", ((MSTestMember)test).Member.FullName);
			} else if (test is MSTestClass) {
				commandLine.Append("test", ((MSTestClass)test).GetTypeName());
			}
			return commandLine.ToString();
		}
		
		public ProcessStartInfo ProcessStartInfo { get; private set; }
	}
}
