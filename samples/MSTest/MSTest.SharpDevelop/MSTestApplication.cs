// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
