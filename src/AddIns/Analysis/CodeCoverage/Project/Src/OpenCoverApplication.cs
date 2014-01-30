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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	public class OpenCoverApplication
	{
		string fileName = String.Empty;
		ProcessStartInfo targetProcessStartInfo;
		OpenCoverSettings settings;
		StringBuilder arguments;
		IProject project;
		
		public OpenCoverApplication(
			string fileName,
			ProcessStartInfo targetProcessStartInfo,
			OpenCoverSettings settings,
			IProject project)
		{
			this.fileName = fileName;
			this.targetProcessStartInfo = targetProcessStartInfo;
			this.settings = settings;
			this.project = project;
			
			if (String.IsNullOrEmpty(fileName)) {
				GetOpenCoverApplicationFileName();
			}
		}
		
		public OpenCoverApplication(
			ProcessStartInfo targetProcessStartInfo,
			OpenCoverSettings settings,
			IProject project)
			: this(null, targetProcessStartInfo, settings, project)
		{
		}
		
		void GetOpenCoverApplicationFileName()
		{
			fileName = Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\OpenCover\OpenCover.Console.exe");
			fileName = Path.GetFullPath(fileName);
		}
		
		public OpenCoverSettings Settings {
			get { return settings; }
		}
		
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public string Target {
			get { return targetProcessStartInfo.FileName; }
		}
		
		public string GetTargetArguments()
		{
			return targetProcessStartInfo.Arguments;
		}
		
		public string GetTargetWorkingDirectory()
		{
			return Path.GetDirectoryName(project.OutputAssemblyFullPath);
		}
		
		public string CodeCoverageResultsFileName {
			get { return new ProjectCodeCoverageResultsFileName(project).FileName; }
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.EnvironmentVariables.Add("COMPLUS_ProfAPI_ProfilerCompatibilitySetting", "EnableV2Profiler");
			processStartInfo.FileName = FileName;
			processStartInfo.Arguments = GetArguments();
			return processStartInfo;
		}
		
		string GetArguments()
		{
			// IMPORTANT: https://github.com/sawilde/opencover/wiki/Usage
			arguments = new StringBuilder("-register:user ");
			
			AppendTarget();
			AppendTargetWorkingDirectory();
			AppendTargetArguments();
			AppendCodeCoverageResultsFileName();
			AppendFilter();
			return arguments.ToString().Trim();
		}
		
		void AppendTarget()
		{
			arguments.AppendFormat("-target:\"{0}\" ", Target);
		}
		
		void AppendTargetWorkingDirectory()
		{
			arguments.AppendFormat("-targetdir:\"{0}\" ", GetTargetWorkingDirectory());
		}
		
		void AppendTargetArguments()
		{
			string targetArguments = GetTargetArguments();
			arguments.AppendFormat("-targetargs:\"{0}\" ", targetArguments.Replace("\"", "\\\""));
		}
		
		void AppendCodeCoverageResultsFileName()
		{
			arguments.AppendFormat("-output:\"{0}\" ", CodeCoverageResultsFileName);
		}
		
		void AppendFilter()
		{
			arguments.Append("-filter:\"");
			AppendIncludedItems();
			AppendExcludedItems();
			arguments.Append("\"");
		}
		
		void AppendIncludedItems()
		{
			StringCollection includedItems = settings.Include;
			if (includedItems.Count == 0) {
				includedItems.Add("[*]*");
			}
			AppendItems("+", includedItems);
		}
		
		void AppendExcludedItems()
		{
			AppendEmptySpace();
			AppendItems("-", settings.Exclude);
		}
		
		void AppendEmptySpace()
		{
			arguments.Append(' ');
		}
		
		void AppendItems(string optionName, StringCollection items)
		{
			string itemArgs = GetItemArguments(optionName, items);
			arguments.Append(itemArgs);
		}
		
		string GetItemArguments(string optionName, StringCollection items)
		{
			StringBuilder itemArgs = new StringBuilder();
			foreach (string item in items) {
				itemArgs.AppendFormat("{0}{1} ", optionName, item);
			}
			return itemArgs.ToString().Trim();
		}
	}
}
