// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Helper to run the unit testing console application.
	/// </summary>
	public class UnitTestApplicationStartHelper
	{
		/// <summary>
		/// returns full/path/to/Tools/NUnit
		/// </summary>
		public static string UnitTestApplicationDirectory {
			get {
				return Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\NUnit");
			}
		}
		
		/// <summary>
		/// returns full/path/to/Tools/NUnit/nunit-console.exe (or whichever nunit-console exe is right
		/// for the project - there are different .exes for .NET 4.0 and for x86-only projects.
		/// </summary>
		public string UnitTestApplication {
			get {
				string exe = "nunit-console";
				if (ProjectUsesDotnet20Runtime(project)) {
					exe += "-dotnet2";
				}
				if (IsPlatformTarget32Bit(project)) {
					exe += "-x86";
				}
				exe += ".exe";
				return Path.Combine(UnitTestApplicationDirectory, exe);
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
		/// Fixture to test. Null = test all fixtures.
		/// </summary>
		public string Fixture;
		
		/// <summary>
		/// Test to run. Null = run all tests. Only valid together with the Fixture property.
		/// </summary>
		public string Test;
		
		/// <summary>
		/// File to write test results to.
		/// </summary>
		public string Results;
		
		/// <summary>
		/// The namespace that tests need to be a part of if they are to
		/// be run.
		/// </summary>
		public string NamespaceFilter;
		
		IProject project;

		public void Initialize(IProject project, IClass fixture, IMember test)
		{
			Initialize(project, null, fixture, test);
		}
		
		public void Initialize(IProject project, string namespaceFilter)
		{
			Initialize(project, namespaceFilter, null, null);
		}
		
		public void Initialize(IProject project, string namespaceFilter, IClass fixture, IMember test)
		{
			this.project = project;
			Assemblies.Add(project.OutputAssemblyFullPath);
			if (namespaceFilter != null) {
				NamespaceFilter = namespaceFilter;
			}
			if (fixture != null) {
				Fixture = fixture.DotNetName;
				if (test != null) {
					Test = test.Name;
				}
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		/// <summary>
		/// Gets the full command line to run the unit test application.
		/// This is the combination of the UnitTestApplication and
		/// the command line arguments.
		/// </summary>
		public string GetCommandLine()
		{
			return String.Concat("\"", UnitTestApplication, "\" ", GetArguments());
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
			if (XmlOutputFile != null) {
				b.Append(" /xml=\"");
				b.Append(XmlOutputFile);
				b.Append('"');
			}
			if (Results != null) {
				b.Append(" /results=\"");
				b.Append(Results);
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
		/// Checks that the project's PlatformTarget is x86 for the active configuration.
		/// </summary>
		bool IsPlatformTarget32Bit(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null) {
				string platformTarget = msbuildProject.GetEvaluatedProperty("PlatformTarget");
				return String.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		bool ProjectUsesDotnet20Runtime(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null) {
				string targetFrameworkVersion = msbuildProject.GetEvaluatedProperty("TargetFrameworkVersion");
				return !String.Equals(targetFrameworkVersion, "v4.0", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}
}
