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

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace VBNetBinding
{
	public class VBNetLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C#";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		#region routines for single file compilation
		public bool CanCompile(string fileName)
		{
			Debug.Assert(fileName != null);
			
			string ext = Path.GetExtension(fileName);
			if (ext == null) {
				return false;
			}
			return ext.ToUpper() == ".CS";
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			Debug.Assert(CanCompile(fileName));
			
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public CompilerResults CompileFile(string fileName)
		{
			Debug.Assert(CanCompile(fileName));
			
			// TODO: Implement me!
			return null;
		}
		
		public void Execute(string fileName, bool debug)
		{
			string exe = GetCompiledOutputName(fileName);
			
			
			if (debug) {
				DebuggerService.Start(exe, Path.GetDirectoryName(exe), "");
			} else {
				ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
				psi.WorkingDirectory = Path.GetDirectoryName(exe);
				psi.UseShellExecute = false;
				
				DebuggerService.StartWithoutDebugging(psi);
			}
		}
		#endregion
		
		public IProject LoadProject(string fileName, string projectName)
		{
			return new VBNetProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new VBNetProject(info);
		}
	}
}
