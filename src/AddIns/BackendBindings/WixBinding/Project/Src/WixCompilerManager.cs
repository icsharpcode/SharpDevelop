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

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Description of WixCompilerManager.	
	/// </summary>
	public class WixCompilerManager
	{
		
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".msi");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			WixProject p = (WixProject)project;
			WixCompilerParameters compilerparameters = (WixCompilerParameters)p.ActiveConfiguration;
			string exe  = FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".msi";
			return exe;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".WXS";
		}
		
		ICompilerResult Compile(WixCompilerParameters compilerparameters, string[] fileNames)
		{
			string output = String.Empty;
			string error  = String.Empty;
			
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);
			
			
			writer.WriteLine("-dDATADIR=\"" + PropertyService.DataDirectory + "\"");
			foreach (string fileName in fileNames) {
				writer.WriteLine("\"" + Path.GetFullPath(fileName) + "\"");
			}
			
			string wixobj  = Path.GetFullPath(FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".wixobj");
			writer.WriteLine("-out \"" + wixobj + "\"");
			
			writer.Close();
			
			string compilerName = GetCompilerName();
			string outstr = compilerName + " \"@"  + responseFileName + "\"";
			
			TempFileCollection tf = new TempFileCollection();
			Executor.ExecWaitWithCapture(outstr, Path.GetFullPath(compilerparameters.OutputDirectory), tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			Console.WriteLine(result.CompilerOutput);
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		ICompilerResult Link(WixCompilerParameters compilerparameters, string[] fileNames)
		{
			string output = String.Empty;
			string error  = String.Empty;
			
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);
			
			string wixobj  = Path.GetFullPath(FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".wixobj");
			writer.WriteLine("\"" + wixobj + "\"");
			string exe = Path.ChangeExtension(wixobj, ".msi");
			writer.WriteLine("-out \"" + exe + "\"");
			writer.Close();
			
			string linkerName = GetLinkerName();
			string outstr = linkerName + " \"@"  + responseFileName + "\"";
			Console.WriteLine(Path.GetFullPath(compilerparameters.OutputDirectory));
			TempFileCollection tf = new TempFileCollection();
			Executor.ExecWaitWithCapture(outstr, Path.GetFullPath(compilerparameters.OutputDirectory), tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		public ICompilerResult CompileFile(string fileName, WixCompilerParameters compilerparameters)
		{
			compilerparameters.OutputDirectory = Path.GetDirectoryName(fileName);
			compilerparameters.OutputAssembly  = Path.GetFileNameWithoutExtension(fileName);
			
			ICompilerResult result = Compile(compilerparameters, new string[] { fileName });
			if (result.CompilerResults.Errors.Count > 0) {
				return result;
			}
			ICompilerResult linkResult = Link(compilerparameters, new string[] { fileName });
			return new DefaultCompilerResult(linkResult.CompilerResults, result.CompilerOutput + linkResult.CompilerOutput);
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			WixProject            p                  = (WixProject)project;
			WixCompilerParameters compilerparameters = (WixCompilerParameters)p.ActiveConfiguration;
			
			ArrayList fileNames = new ArrayList();
			
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							fileNames.Add(finfo.Name);
							break;
//						case BuildAction.EmbedAsResource:
//							writer.WriteLine("\"/res:" + finfo.Name + "\"");
//							break;
					}
				}
			}
			
			ICompilerResult result = Compile(compilerparameters, (string[])fileNames.ToArray(typeof(string)));
			if (result.CompilerResults.Errors.Count > 0) {
				return result;
			}
			return Link(compilerparameters, (string[])fileNames.ToArray(typeof(string)));
		}
		
		string GetCompilerName()
		{
			return Path.Combine(Path.Combine(Application.StartupPath, "wix"), "candle.exe");
		}
		
		string GetLinkerName()
		{
			return Path.Combine(Path.Combine(Application.StartupPath, "wix"), "light.exe");
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			StringBuilder compilerOutput = new StringBuilder();
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
//			// we have 2 formats for the error output the csc gives :
//			Regex normalError  = new Regex(@"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
//			Regex generalError = new Regex(@"(?<error>.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
//			
			while (true) {
				string curLine = sr.ReadLine();
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
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
