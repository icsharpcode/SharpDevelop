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

namespace CPPBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class CPPBindingExecutionManager
	{
		public void Execute(string filename, bool debug)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = false;
			try {
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + exe + "\"\n(.NET bug? Try restaring SD or manual start)");
			}
		}
		
		public void Execute(IProject project, bool debug)
		{
			CPPCompilerParameters parameters = (CPPCompilerParameters)project.ActiveConfiguration;
			
			
			string exe = ((CPPCompilerParameters)project.ActiveConfiguration).OutputFile;
			
			//string args = ((CPPCompilerParameters)project.ActiveConfiguration).CommandLineParameters;
			string args = "";
			
			ProcessStartInfo psi;
			string runtimeStarter = String.Empty;
			
			psi = new ProcessStartInfo(runtimeStarter + "\"" + exe + "\"");
			psi.Arguments = args;
			
			try {
				psi.WorkingDirectory = Path.GetDirectoryName(exe);
				psi.UseShellExecute  =  false;
				
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + exe + "\"");
			}
		}
	}
}
