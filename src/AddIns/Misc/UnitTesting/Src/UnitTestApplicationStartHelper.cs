// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		public const string TargetFrameworkVersionNet11 = "v1.1";

		/// <summary>
		/// returns full/path/to/Tools/NUnit
		/// </summary>
		public static string UnitTestApplicationDirectory {
			get {
				return Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\NUnit");
			}
		}
		
		/// <summary>
		/// returns full/path/to/Tools/NUnit that runs under .NET 1.1.
		/// </summary>
		public static string UnitTestApplicationDirectoryNet11 {
			get {
				return Path.Combine(UnitTestApplicationDirectory, "Net-1.1");
			}
		}

		/// <summary>
		/// returns full/path/to/Tools/NUnit/nunit-console.exe
		/// </summary>
		public static string UnitTestConsoleApplication {
			get {
				return Path.Combine(UnitTestApplicationDirectory, "nunit-console.exe");
			}
		}
		
		/// <summary>
		/// returns full/path/to/Tools/NUnit/nunit-console.exe that runs under .NET 1.1.
		/// </summary>
		public static string UnitTestConsoleApplicationNet11 {
			get {
				return Path.Combine(UnitTestApplicationDirectoryNet11, "nunit-console.exe");
			}
		}
		
		public readonly List<string> Assemblies = new List<string>();
		
		/// <summary>
		/// Use shadow copy assemblies. Default = true.
		/// </summary>
		public bool ShadowCopy = true;
		
		/// <summary>
		/// Run tests on separate thread. Default = false.
		/// </summary>
		public bool Threaded = false;
		
		/// <summary>
		/// Use /nologo directive.
		/// </summary>
		public bool NoLogo = false;
		
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
				Fixture = fixture.FullyQualifiedName;
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
		/// Gets the Unit Test console application filename based on the
		/// target framework specified in the project.
		/// </summary>
		/// <remarks>Deliberately using the unevaluated property since the
		/// SharpDevelop build targets file changes the target version to
		/// v1.0 if it is v1.1.</remarks>
		public static string GetUnitTestConsoleApplication(string targetFrameworkVersion)
		{
			switch (targetFrameworkVersion) {
				case TargetFrameworkVersionNet11:
					return UnitTestConsoleApplicationNet11;
				default:
					return UnitTestConsoleApplication;
			}
		}
		
		/// <summary>
		/// Gets the Unit Test console application filename based on the
		/// target framework specified in the project.
		/// </summary>
		/// <remarks>Deliberately using the unevaluated property since the
		/// SharpDevelop build targets file changes the target version to
		/// v1.0 if it is v1.1.</remarks>
		public string GetUnitTestConsoleApplication()
		{
			MSBuildBasedProject msbuildBasedProject = (MSBuildBasedProject)project;
			string targetFrameworkVersion = msbuildBasedProject.GetUnevalatedProperty("TargetFrameworkVersion");
			return GetUnitTestConsoleApplication(targetFrameworkVersion);
		}
		
		/// <summary>
		/// Gets the full command line to run the unit test application.
		/// This is the combination of the UnitTestConsoleApplication and
		/// the command line arguments.
		/// </summary>
		public string GetCommandLine()
		{
			return String.Concat("\"", GetUnitTestConsoleApplication(), "\" ", GetArguments());
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
			if (Threaded)
				b.Append(" /thread");
			if (NoLogo)
				b.Append(" /nologo");
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
			if (NamespaceFilter != null) {
				b.Append(" /namespaceFilter=\"");
				b.Append(NamespaceFilter);
				b.Append('"');
			}
			if (Fixture != null) {
				b.Append(" /fixture=\"");
				b.Append(Fixture);
				b.Append('"');
				if (Test != null) {
					b.Append(" /testMethodName=\"");
					b.Append(Fixture);
					b.Append('.');
					b.Append(Test);
					b.Append('"');
				}
			}
			return b.ToString();
		}
	}
}
