// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestRunnerApplication
	{
		string testResultsFileName = String.Empty;
		RubyAddInOptions options;
		RubyTestRunnerResponseFile responseFile;
		IRubyFileService fileService;
		CreateTextWriterInfo textWriterInfo;
		RubyConsoleApplication consoleApplication;
		StringBuilder arguments;

		public RubyTestRunnerApplication(string testResultsFileName,
			RubyAddInOptions options,
			IRubyFileService fileService)
		{
			this.testResultsFileName = testResultsFileName;
			this.options = options;
			this.fileService = fileService;
			consoleApplication = new RubyConsoleApplication(options);
		}
		
		public bool Debug {
			get { return consoleApplication.Debug; }
			set { consoleApplication.Debug = value; }
		}
		
		public void CreateResponseFile(SelectedTests selectedTests)
		{
			CreateResponseFile();
			using (responseFile) {
				WriteTests(selectedTests);
			}
		}
		
		void CreateResponseFile()
		{
			TextWriter writer = CreateTextWriter();
			responseFile = new RubyTestRunnerResponseFile(writer);
		}
		
		TextWriter CreateTextWriter()
		{
			string fileName = fileService.GetTempFileName();
			textWriterInfo = new CreateTextWriterInfo(fileName, Encoding.ASCII, false);
			return fileService.CreateTextWriter(textWriterInfo);
		}
		
		void WriteTests(SelectedTests selectedTests)
		{
			responseFile.WriteTests(selectedTests);
		}
		
		public ProcessStartInfo CreateProcessStartInfo(SelectedTests selectedTests)
		{
			consoleApplication.RubyScriptFileName = GetSharpDevelopTestRubyScriptFileName();
			AddLoadPaths();
			consoleApplication.RubyScriptCommandLineArguments = GetCommandLineArguments(selectedTests);
			consoleApplication.WorkingDirectory = selectedTests.Project.Directory;
			return consoleApplication.GetProcessStartInfo();
		}
		
		void AddLoadPaths()
		{
			if (options.HasRubyLibraryPath) {
				consoleApplication.AddLoadPath(options.RubyLibraryPath);
			}
			string testRunnerLoadPath = Path.GetDirectoryName(consoleApplication.RubyScriptFileName);
			consoleApplication.AddLoadPath(testRunnerLoadPath);
		}
		
		string GetSharpDevelopTestRubyScriptFileName()
		{
			string fileName = StringParser.Parse(@"${addinpath:ICSharpCode.RubyBinding}\TestRunner\sdtest.rb");
			return Path.GetFullPath(fileName);
		}
		
		string GetCommandLineArguments(SelectedTests selectedTests)
		{
			arguments = new StringBuilder();
			AppendSelectedTest(selectedTests);
			AppendTestResultsFileNameAndResponseFileNameArgs();
			
			return arguments.ToString();
		}
				
		void AppendSelectedTest(SelectedTests selectedTests)
		{
			if (selectedTests.Method != null) {
				AppendSelectedTestMethod(selectedTests.Method);
			} else if (selectedTests.Class != null) {
				AppendSelectedTestClass(selectedTests.Class);
			}
		}
		
		void AppendSelectedTestMethod(IMember method)
		{
			arguments.AppendFormat("--name={0} ", method.Name);
		}
		
		void AppendSelectedTestClass(IClass c)
		{
			arguments.AppendFormat("--testcase={0} ", c.FullyQualifiedName);
		}
		
		void AppendTestResultsFileNameAndResponseFileNameArgs()
		{
			arguments.AppendFormat("-- \"{0}\" \"{1}\"", testResultsFileName, textWriterInfo.FileName);
		}
		
		public void Dispose()
		{
			fileService.DeleteFile(textWriterInfo.FileName);
		}
	}
}
