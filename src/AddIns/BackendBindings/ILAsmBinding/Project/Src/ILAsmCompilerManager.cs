// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.ILAsmBinding
{
	/// <summary>
	/// Description of ILAsmCompilerManager.
	/// </summary>
	public class ILAsmCompilerManager
	{
		
		
		// we have 2 formats for the error output the csc gives :
		readonly static Regex normalError  = new Regex(@"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
		readonly static Regex generalError = new Regex(@"(?<error>.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			ILAsmProject p = (ILAsmProject)project;
			ILAsmCompilerParameters compilerparameters = (ILAsmCompilerParameters)p.ActiveConfiguration;
			string ext;
			if (compilerparameters.CompilationTarget == CompilationTarget.Dll)
				ext = ".dll";
			else if (compilerparameters.CompilationTarget == CompilationTarget.NetModule)
				ext = ".netmodule";
			else
				ext = ".exe";
			string exe = FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly;
			return exe + ext;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".IL";
		}
		
		ICompilerResult Compile(ILAsmCompilerParameters compilerparameters, string[] fileNames, string outputFile)
		{
			// TODO: No response file possible ? @FILENAME seems not to work.
			string output = String.Empty;
			string error  = String.Empty;
			StringBuilder parameters = new StringBuilder();
			foreach (string fileName in fileNames) {
				parameters.Append('"');
				parameters.Append(Path.GetFullPath(fileName));
				parameters.Append("\" ");
			}
			
			parameters.Append("/OUTPUT=\"" + outputFile + "\"");
			parameters.Append(" ");
			parameters.Append(compilerparameters.CurrentCompilerOptions.GenerateOptions());
			string compilerName = GetCompilerName(compilerparameters);
			string outstr = compilerName + " " + parameters.ToString();
			
			TempFileCollection tf = new TempFileCollection();
			Executor.ExecWaitWithCapture(outstr, Path.GetFullPath(compilerparameters.OutputDirectory), tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		public ICompilerResult CompileFile(string fileName, ILAsmCompilerParameters compilerparameters)
		{
			compilerparameters.OutputDirectory = Path.GetDirectoryName(fileName);
			compilerparameters.OutputAssembly  = Path.GetFileNameWithoutExtension(fileName);
			
			return Compile(compilerparameters, new string[] { fileName }, GetCompiledOutputName(fileName));
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			ILAsmProject            p                  = (ILAsmProject)project;
			ILAsmCompilerParameters compilerparameters = (ILAsmCompilerParameters)p.ActiveConfiguration;
			
			ArrayList fileNames = new ArrayList();
			
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							fileNames.Add(finfo.Name);
							break;
						//				TODO : Embedded resources ?
						//						case BuildAction.EmbedAsResource:
						//							writer.WriteLine("\"/res:" + finfo.Name + "\"");
						//							break;
					}
				}
			}
			
			return Compile(compilerparameters, (string[])fileNames.ToArray(typeof(string)), GetCompiledOutputName(project));
		}
		
		string GetCompilerName(ILAsmCompilerParameters compilerParameters)
		{
			if (compilerParameters.ILAsmCompiler == ILAsmCompiler.Mono) {
				return System.Environment.GetEnvironmentVariable("ComSpec") + " /c ilasm";
			}
			string compilerVersion = compilerParameters.ILAsmCompilerVersion;
			string runtimeDirectory = Path.Combine(FileUtility.NETFrameworkInstallRoot, compilerVersion);
			if (compilerVersion.Length == 0 || compilerVersion == "Standard" || !Directory.Exists(runtimeDirectory)) {
				runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			}
			return '"' + Path.Combine(runtimeDirectory, "ilasm.exe") + '"';
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			StringBuilder compilerOutput = new StringBuilder();
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
			while (true) {
				string curLine = sr.ReadLine();
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
				// TODO : PARSE ERROR OUTPUT.
				
				//				curLine = curLine.Trim();
				//				if (curLine.Length == 0) {
				//					continue;
				//				}
				//
				//				CompilerError error = new CompilerError();
				//
				//				// try to match standard errors
				//				Match match = normalError.Match(curLine);
				//				if (match.Success) {
				//					error.Column      = Int32.Parse(match.Result("${column}"));
				//					error.Line        = Int32.Parse(match.Result("${line}"));
				//					error.FileName    = Path.GetFullPath(match.Result("${file}"));
				//					error.IsWarning   = match.Result("${error}") == "warning";
				//					error.ErrorNumber = match.Result("${number}");
				//					error.ErrorText   = match.Result("${message}");
				//				} else {
				//					match = generalError.Match(curLine); // try to match general csc errors
				//					if (match.Success) {
				//						error.IsWarning   = match.Result("${error}") == "warning";
				//						error.ErrorNumber = match.Result("${number}");
				//						error.ErrorText   = match.Result("${message}");
				//					} else { // give up and skip the line
				//						continue;
				////						error.IsWarning = false;
				////						error.ErrorText = curLine;
				//					}
				//				}
				//
				//				cr.Errors.Add(error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
		}
	}
}
