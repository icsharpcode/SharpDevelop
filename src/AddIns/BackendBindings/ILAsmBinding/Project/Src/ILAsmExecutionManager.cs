// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
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

namespace ICSharpCode.ILAsmBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class ILAsmExecutionManager
	{
		public void Execute(string filename, bool debug)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo("\"" + exe + "\"");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = true;
			
			DebuggerService DebuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			DebuggerService.StartWithoutDebugging(psi);
		}
		
		public void Execute(IProject project, bool debug)
		{
			ILAsmCompilerParameters parameters = (ILAsmCompilerParameters)project.ActiveConfiguration;
			if (parameters.CompilationTarget != CompilationTarget.Exe) {
				
				MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
				return;
			}
			
			
			string exe = Path.GetFullPath(Path.Combine(parameters.OutputDirectory, parameters.OutputAssembly) + ".exe");
			
			ProcessStartInfo psi;
			switch (parameters.NetRuntime) {
				case NetRuntime.Mono:
					psi = new ProcessStartInfo("mono", "\"" + exe + "\"");
					break;
				case NetRuntime.MonoInterpreter:
					psi = new ProcessStartInfo("mint", "\"" + exe + "\"");
					break;
				default:
					psi = new ProcessStartInfo("\"" + exe + "\"");
					break;
			}
			
			//if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput) {
			//	psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + runtimeStarter + "\"" + exe + "\" " + args +  " & pause");
			// } else {
			//	psi = new ProcessStartInfo(runtimeStarter + "\"" + exe + "\"");
			//	psi.Arguments = args;
			//}
			
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute  = true;
			
			DebuggerService DebuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			DebuggerService.StartWithoutDebugging(psi);
		}
	}
}
