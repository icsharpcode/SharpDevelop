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
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
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

			string filterFileName = CreateFilterFile();
			if (filterFileName != null) {
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
			IList<string> filter = classFilterBuilder.BuildFilterFor(tests);
			
			string path = null;
			if (filter.Count > 0) {
				path = Path.GetTempFileName();
				using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
				using (var writer = new StreamWriter(stream)) {
					foreach (string testClassName in filter) {
						writer.WriteLine(testClassName);
					}
				}
			}
			return path;
		}

		string ExecutableFileName {
			get {
				string assemblyDirectory = Path.GetDirectoryName(new Uri(typeof(MSpecApplication).Assembly.CodeBase).LocalPath);
				string runnerDirectory = Path.Combine(assemblyDirectory, @"Tools\Machine.Specifications");

				string executableName = "mspec";
				if (project.IsPlatformTarget32BitOrAnyCPU())
					executableName += "-x86";
				if (!ProjectUsesDotnet20Runtime(project))
					executableName += "-clr4";

				executableName += ".exe";
				return Path.Combine(runnerDirectory, executableName);
			}
		}

		static bool ProjectUsesDotnet20Runtime(IProject project)
		{
			var p = project as ICSharpCode.SharpDevelop.Project.Converter.IUpgradableProject;
			if (p != null && p.CurrentTargetFramework != null) {
				return p.CurrentTargetFramework.SupportedRuntimeVersion == "v2.0.50727";
			}
			return false;
		}

		string WorkingDirectory {
			get {
				return Path.GetDirectoryName(project.OutputAssemblyFullPath);
			}
		}
	}
}
