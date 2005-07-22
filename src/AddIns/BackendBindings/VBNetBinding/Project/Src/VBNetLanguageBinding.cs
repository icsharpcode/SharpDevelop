// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
		public const string LanguageName = "VBNet";
		
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
			return ext.ToUpper() == ".VB";
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
				ProcessStartInfo psi = new ProcessStartInfo();
				psi.FileName = exe;
				psi.WorkingDirectory = Path.GetDirectoryName(exe);
				psi.Arguments = "";

				DebuggerService.CurrentDebugger.Start(psi);
			} else {
				ProcessStartInfo psi = new ProcessStartInfo();
				psi.FileName = Environment.GetEnvironmentVariable("ComSpec");
				psi.WorkingDirectory = Path.GetDirectoryName(exe);
				psi.Arguments = "/c " + "\"" + exe + "\"" + " & pause";
				psi.UseShellExecute = false;
				
				DebuggerService.CurrentDebugger.StartWithoutDebugging(psi);
			}
		}
		#endregion
		
		public IProject LoadProject(string fileName, string projectName)
		{
			return new VBNetProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			VBNetProject p = new VBNetProject(info);
			if (projectOptions != null) {
				p.ImportOptions(projectOptions.Attributes);
			}
			return p;
		}
	}
}
