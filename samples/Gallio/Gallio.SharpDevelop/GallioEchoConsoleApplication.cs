// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;

namespace Gallio.SharpDevelop
{
	public class GallioEchoConsoleApplication
	{
		string fileName = String.Empty;
		SelectedTests selectedTests;
		StringBuilder commandLine = new StringBuilder();
		List<TestRunnerExtensionCommandLineArgument> testRunnerExtensions = 
			new List<TestRunnerExtensionCommandLineArgument>();
		
		public GallioEchoConsoleApplication(SelectedTests selectedTests, string fileName)
		{
			this.selectedTests = selectedTests;
			this.fileName = fileName;
		}
		
		public GallioEchoConsoleApplication(SelectedTests selectedTests)
			: this(selectedTests, String.Empty)
		{
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public List<TestRunnerExtensionCommandLineArgument> TestRunnerExtensions {
			get { return testRunnerExtensions; }
		}
		
		public string GetArguments()
		{
			AppendDotNet4Framework();
			AppendTestRunnerExtensions();
			AppendAssemblyToTest();
			
			return commandLine.ToString().TrimEnd();
		}
		
		void AppendDotNet4Framework()
		{
			AppendArgument("/rv:v4.0.30319");

		}
		
		void AppendTestRunnerExtensions()
		{
			foreach (TestRunnerExtensionCommandLineArgument arg in testRunnerExtensions) {
				AppendArgument(arg.ToString());
			}
		}
		
		void AppendArgument(string argument)
		{
			commandLine.Append(argument);
			commandLine.Append(' ');
		}
		
		void AppendAssemblyToTest()
		{
			AppendQuoted(selectedTests.Project.OutputAssemblyFullPath);
		}
		
		void AppendQuoted(string argument)
		{
			commandLine.AppendFormat("\"{0}\"", argument);
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = fileName;
			startInfo.Arguments = GetArguments();
			startInfo.WorkingDirectory = GetWorkingDirectory();
			return startInfo;
		}
		
		string GetWorkingDirectory()
		{
			return StringParser.Parse("${addinpath:ICSharpCode.Gallio}");
		}
	}
}
