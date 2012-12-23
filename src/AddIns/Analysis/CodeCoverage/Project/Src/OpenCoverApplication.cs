// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		NUnitConsoleApplication nunitConsoleApp;
		OpenCoverSettings settings;
		StringBuilder arguments;
		
		public OpenCoverApplication(string fileName, NUnitConsoleApplication nunitConsoleApp, OpenCoverSettings settings)
		{
			this.fileName = fileName;
			this.nunitConsoleApp = nunitConsoleApp;
			this.settings = settings;
			
			if (String.IsNullOrEmpty(fileName)) {
				GetPartCoverApplicationFileName();
			}
		}
		
		public OpenCoverApplication(NUnitConsoleApplication nunitConsoleApp, OpenCoverSettings settings)
			: this(null, nunitConsoleApp, settings)
		{
		}
		
		void GetPartCoverApplicationFileName()
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
			get { return nunitConsoleApp.FileName; }
		}
		
		public string GetTargetArguments()
		{
			return nunitConsoleApp.GetArguments();
		}
		
		public string GetTargetWorkingDirectory()
		{
			return Path.GetDirectoryName(nunitConsoleApp.Assemblies[0]);
		}
		
		public string CodeCoverageResultsFileName {
			get { return new ProjectCodeCoverageResultsFileName(nunitConsoleApp.Project).FileName; }
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
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
