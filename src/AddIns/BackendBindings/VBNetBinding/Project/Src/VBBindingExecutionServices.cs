// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
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

namespace VBBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class VBBindingExecutionServices
	{	

		public void Execute(string filename, bool debug)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			DebuggerService DebuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			if (debug) {
				DebuggerService.Start(exe, Path.GetDirectoryName(exe), "");
			} else {
				ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
				psi.WorkingDirectory = Path.GetDirectoryName(exe);
				psi.UseShellExecute = false;
				
				DebuggerService.StartWithoutDebugging(psi);
			}
		}
		
		public void Execute(IProject project, bool debug)
		{
			VBCompilerParameters parameters = (VBCompilerParameters)project.ActiveConfiguration;
			
			
			string directory = FileUtility.GetDirectoryNameWithSeparator(parameters.OutputDirectory);
			string exe = parameters.OutputAssembly + ".exe";
			string args = parameters.CommandLineParameters;

			ProcessStartInfo psi;
//			bool customStartup = false;
			if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput && !debug) {
//				customStartup = true;
				psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c \"" + directory + exe + "\" " + args +  " & pause");
			} else {
				if (parameters.CompileTarget == CompileTarget.Library) {
					
					MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
					return;
				}
			
				psi = new ProcessStartInfo(directory + exe);
				psi.Arguments = args;
			}
			
			psi.WorkingDirectory = Path.GetDirectoryName(directory);
			psi.UseShellExecute = false;
			DebuggerService DebuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			if (debug /*&& !customStartup*/) {
				DebuggerService.Start(Path.Combine(directory, exe), directory, args);
			} else {
				DebuggerService.StartWithoutDebugging(psi);
			}
		}
	}
}
