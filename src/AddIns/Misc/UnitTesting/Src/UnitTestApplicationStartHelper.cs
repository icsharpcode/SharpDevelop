// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				return Path.Combine(FileUtility.ApplicationRootPath, "bin/Tools/NUnit");
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
		
		public void Initialize(IProject project, IClass fixture, IMember test)
		{
			Assemblies.Add(project.OutputAssemblyFullPath);
			if (fixture != null) {
				Fixture = fixture.FullyQualifiedName;
				if (test != null) {
					Test = test.Name;
				}
			}
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
		
		public void DisplayResults()
		{
			DisplayResults(XmlOutputFile);
		}
		
		public static void DisplayResults(string resultFile)
		{
			if (resultFile == null)
				throw new ArgumentNullException("resultFile");
			
			if (!File.Exists(resultFile)) {
				Task task = new Task("", "No NUnit results file generated: " + resultFile, 0, 0, TaskType.Error);
				WorkbenchSingleton.SafeThreadAsyncCall(typeof(TaskService), "Add", task);
				return;
			}
			
			try {
				NUnitResults results = new NUnitResults(resultFile);
				WorkbenchSingleton.SafeThreadAsyncCall(typeof(TaskService), "AddRange", results.Tasks);
			} catch (System.Xml.XmlException ex) {
				Task task = new Task(resultFile, "Invalid NUnit results file: " + ex.Message, ex.LineNumber, ex.LinePosition, TaskType.Error);
				WorkbenchSingleton.SafeThreadAsyncCall(typeof(TaskService), "Add", task);
			}
		}
	}
}
