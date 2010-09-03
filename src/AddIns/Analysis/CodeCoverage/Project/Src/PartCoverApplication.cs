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
	public class PartCoverApplication
	{
		string fileName = String.Empty;
		NUnitConsoleApplication nunitConsoleApp;
		PartCoverSettings settings;
		StringBuilder arguments;
		
		public PartCoverApplication(string fileName, NUnitConsoleApplication nunitConsoleApp, PartCoverSettings settings)
		{
			this.fileName = fileName;
			this.nunitConsoleApp = nunitConsoleApp;
			this.settings = settings;
			
			if (String.IsNullOrEmpty(fileName)) {
				GetPartCoverApplicationFileName();
			}
		}
		
		public PartCoverApplication(NUnitConsoleApplication nunitConsoleApp, PartCoverSettings settings)
			: this(null, nunitConsoleApp, settings)
		{
		}
		
		void GetPartCoverApplicationFileName()
		{
			fileName = Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\PartCover\PartCover.exe");
			fileName = Path.GetFullPath(fileName);
		}
		
		public PartCoverSettings Settings {
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
			get { return GetCodeCoverageResultsFileName(); }
		}
		
		string GetCodeCoverageResultsFileName()
		{
			string  outputDirectory = GetOutputDirectory(nunitConsoleApp.Project);
			return Path.Combine(outputDirectory, "coverage.xml");
		}
		
		string GetOutputDirectory(IProject project)
		{
			return Path.Combine(project.Directory, "PartCover");
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
			arguments = new StringBuilder();
			
			AppendTarget();
			AppendTargetWorkingDirectory();
			AppendTargetArguments();
			AppendCodeCoverageResultsFileName();
			AppendIncludedItems();
			AppendExcludedItems();
			
			return arguments.ToString().Trim();
		}
		
		void AppendTarget()
		{
			arguments.AppendFormat("--target \"{0}\" ", Target);
		}
		
		void AppendTargetWorkingDirectory()
		{
			arguments.AppendFormat("--target-work-dir \"{0}\" ", GetTargetWorkingDirectory());
		}
		
		void AppendTargetArguments()
		{
			string targetArguments = GetTargetArguments();
			arguments.AppendFormat("--target-args \"{0}\" ", targetArguments.Replace("\"", "\\\""));
		}
		
		void AppendCodeCoverageResultsFileName()
		{
			arguments.AppendFormat("--output \"{0}\" ", CodeCoverageResultsFileName);
		}
		
		void AppendIncludedItems()
		{
			StringCollection includedItems = settings.Include;
			if (includedItems.Count == 0) {
				includedItems.Add("[*]*");
			}
			AppendItems("--include", includedItems);
		}
		
		void AppendExcludedItems()
		{
			AppendEmptySpace();
			AppendItems("--exclude", settings.Exclude);
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
				itemArgs.AppendFormat("{0} {1} ", optionName, item);
			}
			return itemArgs.ToString().Trim();
		}
	}
}
