// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace CPPBinding
{
	public class CPPLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C++.NET";
		
		CPPBindingCompilerManager   compilerManager  = new CPPBindingCompilerManager();
		CPPBindingExecutionManager  executionManager = new CPPBindingExecutionManager();
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public void Execute(string filename, bool debug)
		{
			Debug.Assert(executionManager != null);
			executionManager.Execute(filename, debug);
		}
		
		public void Execute(IProject project, bool debug)
		{
			Debug.Assert(executionManager != null);
			executionManager.Execute(project, debug);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.GetCompiledOutputName(fileName);
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.GetCompiledOutputName(project);
		}
		
		public bool CanCompile(string fileName)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.CanCompile(fileName);
		}
		
		public ICompilerResult CompileFile(string fileName)
		{
			MessageBox.Show("Cannot compile a single file. Create a project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			return null;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			return CompileProject(project, false);
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			return CompileProject(project, true);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new CPPProject(info, projectOptions);
		}
		
		private ICompilerResult CompileProject(IProject project, bool force)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.CompileProject(project, force);
		}
	}
}
