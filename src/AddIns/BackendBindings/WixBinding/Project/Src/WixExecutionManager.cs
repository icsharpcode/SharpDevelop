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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class WixExecutionManager
	{
		public void Execute(string filename, bool debug)
		{
			string exe = Path.ChangeExtension(filename, ".msi");
			ProcessStartInfo psi = new ProcessStartInfo("\"" + exe + "\"");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = true;
			
			DebuggerService DebuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			DebuggerService.StartWithoutDebugging(psi);
		}
		
		public void Execute(IProject project, bool debug)
		{
			WixCompilerParameters parameters = (WixCompilerParameters)project.ActiveConfiguration;
			
			
			string exe = Path.GetFullPath(Path.Combine(parameters.OutputDirectory, parameters.OutputAssembly) + ".msi");
			Console.WriteLine("EXE: " + exe);
			ProcessStartInfo psi = new ProcessStartInfo("\"" + exe  + "\"");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute  = true;
			
			DebuggerService DebuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			DebuggerService.StartWithoutDebugging(psi);
		}
	}
}
