// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace VBBinding
{
	public class VBLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "VBNET";
		
		VBBindingCompilerServices   compilerServices  = new VBBindingCompilerServices();
		VBBindingExecutionServices  executionServices = new VBBindingExecutionServices();
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public void Execute(string filename, bool debug)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(filename, debug);
		}
		
		public void Execute(IProject project, bool debug)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(project, debug);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.GetCompiledOutputName(fileName);
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.GetCompiledOutputName(project);
		}
		
		public bool CanCompile(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CanCompile(fileName);
		}
		
		public ICompilerResult CompileFile(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CompileFile(fileName);
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CompileProject(project);
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			return CompileProject(project);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new VBProject(info, projectOptions);
		}
	}
}
