/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 14:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of MSpecApplication.
	/// </summary>
	public class MSpecApplication
	{
		public MSpecApplication(IEnumerable<ITest> tests)
		{
			InitializeFrom(tests);
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			return new ProcessStartInfo {
				FileName = ExecutableFileName,
				Arguments = GetArguments()
			};
		}
	
		public string Results { get;set; }
		
		void InitializeFrom(IEnumerable<ITest> tests)
		{
			this.tests = tests;
			ITest test = tests.FirstOrDefault();
			if (test != null)
				project = test.ParentProject.Project;
		}
		
		IEnumerable<ITest> tests;
		IProject project;

		string GetArguments()
		{
			var builder = new StringBuilder();

			builder.Append("--xml \"");
			builder.Append(FileUtility.GetAbsolutePath(Environment.CurrentDirectory, Results));
			builder.Append("\" ");

			var filterFileName = CreateFilterFile();
			if (filterFileName != null)
			{
				builder.Append("-f \"");
				builder.Append(FileUtility.GetAbsolutePath(Environment.CurrentDirectory, filterFileName));
				builder.Append("\" ");
			}

			builder.Append("\"");
			builder.Append(project.OutputAssemblyFullPath);
			builder.Append("\"");

			return builder.ToString();
		}

		string CreateFilterFile()
		{
			var classFilterBuilder = new ClassFilterBuilder();
			//var projectContent = SD.ParserService.GetProjectContent(project);
			var filter = classFilterBuilder.BuildFilterFor(tests);
			
			string path = null;
			if (filter.Count > 0) {
				path = Path.GetTempFileName();
				using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
				using (var writer = new StreamWriter(stream))
					foreach (var testClassName in filter) {
						writer.WriteLine(testClassName);
					}
			}
			return path;
		}

		string ExecutableFileName {
			get {
				var assemblyDirectory = Path.GetDirectoryName(new Uri(typeof(MSpecApplication).Assembly.CodeBase).LocalPath);
				var runnerDirectory = Path.Combine(assemblyDirectory, @"Tools\Machine.Specifications");

				string executableName = "mspec";
				if (TargetPlatformIs32Bit(project))
					executableName += "-x86";
				if (UsesClr4(project))
					executableName += "-clr4";

				executableName += ".exe";
				return Path.Combine(runnerDirectory, executableName);
			}
		}

		private bool UsesClr4(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null)
			{
				string targetFrameworkVersion = msbuildProject.GetEvaluatedProperty("TargetFrameworkVersion");
				return String.Equals(targetFrameworkVersion, "v4.0", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private bool TargetPlatformIs32Bit(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null)
			{
				string platformTarget = msbuildProject.GetEvaluatedProperty("PlatformTarget");
				return String.Compare(platformTarget, "x86", true) == 0;
			}
			return false;
		}

		string WorkingDirectory
		{
			get
			{
				return Path.GetDirectoryName(project.OutputAssemblyFullPath);
			}
		}
	}
}
