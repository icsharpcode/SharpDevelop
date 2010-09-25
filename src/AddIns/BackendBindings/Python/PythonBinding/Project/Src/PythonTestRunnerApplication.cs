// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
