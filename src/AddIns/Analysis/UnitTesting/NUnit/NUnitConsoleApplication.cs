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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class NUnitConsoleApplication
	{
		public NUnitConsoleApplication(IEnumerable<ITest> selectedTests, UnitTestingOptions options)
		{
			Initialize(selectedTests);
			InitializeOptions(options);
		}
		
		public NUnitConsoleApplication(IEnumerable<ITest> selectedTests)
		{
			Initialize(selectedTests);
		}
		
		void Initialize(IEnumerable<ITest> selectedTests)
		{
			// TODO: support running multiple tests
			ITest test = selectedTests.FirstOrDefault();
			this.project = test.ParentProject.Project;
			Assemblies.Add(project.OutputAssemblyFullPath);
			if (test is TestNamespace) {
				NamespaceFilter = ((TestNamespace)test).NamespaceName;
			} else if (test is NUnitTestClass) {
				var testClass = (NUnitTestClass)test;
				Fixture = testClass.ReflectionName;
			} else if (test is NUnitTestMethod) {
				var testMethod = (NUnitTestMethod)test;
				Fixture = testMethod.FixtureReflectionName;
				Test = testMethod.MethodNameWithDeclaringTypeForInheritedTests;
			}
		}
		
		void InitializeOptions(UnitTestingOptions options)
		{
			NoThread = options.NoThread;
			NoLogo = options.NoLogo;
			NoDots = options.NoDots;
			Labels = options.Labels;
			ShadowCopy = !options.NoShadow;
			NoXmlOutputFile = !options.CreateXmlOutputFile;
			
			if (options.CreateXmlOutputFile) {
				GenerateXmlOutputFileName();
			}
		}
		
		void GenerateXmlOutputFileName()
		{
			string directory = Path.GetDirectoryName(project.OutputAssemblyFullPath);
			string fileName = project.AssemblyName + "-TestResult.xml";
			XmlOutputFile = Path.Combine(directory, fileName);
		}
		
		/// <summary>
		/// returns full/path/to/Tools/NUnit
		/// </summary>
		string WorkingDirectory {
			get { return Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\NUnit"); }
		}
				
		/// <summary>
		/// returns full/path/to/Tools/NUnit/nunit-console.exe or nunit-console-x86.exe if the
		/// project platform target is x86.
		/// </summary>
		public string FileName {
			get {
				string exe = "nunit-console";
				if (ProjectUsesDotnet20Runtime(project)) {
					exe += "-dotnet2";
				}
				// As SharpDevelop can't debug 64-bit applications yet, use
				// 32-bit NUnit even for AnyCPU test projects.
				if (IsPlatformTarget32BitOrAnyCPU(project)) {
					exe += "-x86";
				}
				exe += ".exe";
				return Path.Combine(WorkingDirectory, exe);
			}
		}
		
		public readonly List<string> Assemblies = new List<string>();
		
		/// <summary>
		/// Use shadow copy assemblies. Default = true.
		/// </summary>
		public bool ShadowCopy = true;
		
		/// <summary>
		/// Disables the use of a separate thread to run tests on separate thread. Default = false;
		/// </summary>
		public bool NoThread = false;
		
		/// <summary>
		/// Use /nologo directive.
		/// </summary>
		public bool NoLogo = false;
		
		/// <summary>
		/// Use /labels directive.
		/// </summary>
		public bool Labels = false;
		
		/// <summary>
		/// Use /nodots directive.
		/// </summary>
		public bool NoDots = false;
		
		/// <summary>
		/// File to write xml output to. Default = null.
		/// </summary>
		public string XmlOutputFile;
		
		/// <summary>
		/// Use /noxml.
		/// </summary>
		public bool NoXmlOutputFile = true;
		
		/// <summary>
		/// Fixture to test. Null = test all fixtures.
		/// </summary>
		public string Fixture;
		
		/// <summary>
		/// Test to run. Null = run all tests. Only valid together with the Fixture property.
		/// </summary>
		public string Test;
		
		/// <summary>
		/// Pipe to write test results to.
		/// </summary>
		public string ResultsPipe;
		
		/// <summary>
		/// The namespace that tests need to be a part of if they are to 
		/// be run.
		/// </summary>
		public string NamespaceFilter;
		
		IProject project;
		
		public IProject Project {
			get { return project; }
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = FileName;
			startInfo.Arguments = GetArguments();
			startInfo.WorkingDirectory = WorkingDirectory;
			return startInfo;
		}
		
		/// <summary>
		/// Gets the full command line to run the unit test application.
		/// This is the combination of the UnitTestApplication and
		/// the command line arguments.
		/// </summary>
		public string GetCommandLine()
		{
			return String.Format("\"{0}\" {1}", FileName, GetArguments());
		}
		
		/// <summary>
		/// Gets the arguments to use on the command line to run NUnit.
		/// </summary>
		public string GetArguments()
		{
			StringBuilder b = new StringBuilder();
			foreach (string assembly in Assemblies) {
				if (b.Length > 0)
					b.Append(' ');
				b.Append('"');
				b.Append(assembly);
				b.Append('"');
			}
			if (!ShadowCopy)
				b.Append(" /noshadow");
			if (NoThread)
				b.Append(" /nothread");
			if (NoLogo)
				b.Append(" /nologo");
			if (Labels) 
				b.Append(" /labels");
			if (NoDots) 
				b.Append(" /nodots");
			if (NoXmlOutputFile) {
				b.Append(" /noxml");
			} else if (XmlOutputFile != null) {
				b.Append(" /xml=\"");
				b.Append(XmlOutputFile);
				b.Append('"');
			}
			if (ResultsPipe != null) {
				b.Append(" /pipe=\"");
				b.Append(ResultsPipe);
				b.Append('"');
			}
			string run = null;
			if (NamespaceFilter != null) {
				run = NamespaceFilter;
			} else if (Fixture != null) {
				if (Test != null) {
					run = Fixture + "." + Test;
				} else {
					run = Fixture;
				}
			} else if (Test != null) {
				run = Test;
			}
			if (run != null) {
				b.Append(" /run=\"");
				b.Append(run);
				b.Append('"');
			}
			return b.ToString();
		}
		
		/// <summary>
		/// Checks that the project's PlatformTarget is x32 for the active configuration.
		/// </summary>
		static bool IsPlatformTarget32BitOrAnyCPU(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null) {
				string platformTarget = msbuildProject.GetEvaluatedProperty("PlatformTarget");
				return String.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase)
					|| String.Equals(platformTarget, "AnyCPU", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		static bool ProjectUsesDotnet20Runtime(IProject project)
		{
			var p = project as ICSharpCode.SharpDevelop.Project.Converter.IUpgradableProject;
			if (p != null && p.CurrentTargetFramework != null) {
				return p.CurrentTargetFramework.SupportedRuntimeVersion == "v2.0.50727";
			}
			return false;
		}
	}
}
