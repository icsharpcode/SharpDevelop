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
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestRunnerApplication
	{
		string testResultsFileName = String.Empty;
		PythonAddInOptions options;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		PythonTestRunnerResponseFile responseFile;
		IScriptingFileService fileService;
		CreateTextWriterInfo textWriterInfo;
		PythonConsoleApplication consoleApplication;
		
		public PythonTestRunnerApplication(string testResultsFileName,
			PythonAddInOptions options,
			PythonStandardLibraryPath pythonStandardLibraryPath,
			IScriptingFileService fileService)
		{
			this.testResultsFileName = testResultsFileName;
			this.options = options;
			this.pythonStandardLibraryPath = pythonStandardLibraryPath;
			this.fileService = fileService;
			consoleApplication = new PythonConsoleApplication(options);
		}
		
		public bool Debug {
			get { return consoleApplication.Debug; }
			set { consoleApplication.Debug = value; }
		}
		
		public void CreateResponseFile(SelectedTests selectedTests)
		{
			CreateResponseFile();
			using (responseFile) {
				WritePythonSystemPaths();
				WriteTestsResultsFileName();
				WriteTests(selectedTests);
			}
		}
		
		void CreateResponseFile()
		{
			TextWriter writer = CreateTextWriter();
			responseFile = new PythonTestRunnerResponseFile(writer);
		}
		
		TextWriter CreateTextWriter()
		{
			string fileName = fileService.GetTempFileName();
			textWriterInfo = new CreateTextWriterInfo(fileName, Encoding.UTF8, false);
			return fileService.CreateTextWriter(textWriterInfo);
		}
		
		void WritePythonSystemPaths()
		{
			if (options.HasPythonLibraryPath) {
				responseFile.WritePath(options.PythonLibraryPath);
			} else if (pythonStandardLibraryPath.HasPath) {
				responseFile.WritePaths(pythonStandardLibraryPath.Directories);
			}
		}
		
		void WriteTestsResultsFileName()
		{
			responseFile.WriteResultsFileName(testResultsFileName);
		}
		
		void WriteTests(SelectedTests selectedTests)
		{
			responseFile.WriteTests(selectedTests);
		}
		
		public ProcessStartInfo CreateProcessStartInfo(SelectedTests selectedTests)
		{
			consoleApplication.ScriptFileName = GetSharpDevelopTestPythonScriptFileName();
			consoleApplication.ScriptCommandLineArguments = GetResponseFileNameCommandLineArgument();
			consoleApplication.WorkingDirectory = selectedTests.Project.Directory;
			return consoleApplication.GetProcessStartInfo();
		}
		
		string GetSharpDevelopTestPythonScriptFileName()
		{
			return StringParser.Parse(@"${addinpath:ICSharpCode.PythonBinding}\TestRunner\sdtest.py");
		}
		
		string GetResponseFileNameCommandLineArgument()
		{
			return String.Format("\"@{0}\"", textWriterInfo.FileName);
		}
		
		public void Dispose()
		{
			fileService.DeleteFile(textWriterInfo.FileName);
		}
	}
}
