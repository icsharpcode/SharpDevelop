// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms;

namespace CPPBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class CPPBindingCompilerManager
	{	
		private FileUtilityService _fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		private StringBuilder _inputFiles   = new StringBuilder();
		private StringBuilder _buildProcess = new StringBuilder();
		private StringBuilder _results      = new StringBuilder();
		private bool _treatWarningsAsErrors;
		
		
		// we have 2 formats for the error output the csc gives :
		// d:\vc\include\xstring(1466) : warning C4701: local variable '_Ptr' may be used without hav
		readonly static Regex normalError  = new Regex(@"(?<file>.*)\((?<line>\d+)\)\s+\:\s+(?<error>.+?)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
		// cl : Command line error D2016 : '/clr' and '/ML' command-line options are incompatible
		readonly static Regex generalError = new Regex(@"(?<error>.+)\s+(?<number>[\d\w]+)\s*:\s+(?<message>.*)", RegexOptions.Compiled);
			
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)project.ActiveConfiguration;
			return compilerparameters.OutputFile;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName) == ".cpp" || Path.GetExtension(fileName) == ".c" || Path.GetExtension(fileName) == ".cxx";
		}
		
		public ICompilerResult CompileFile(string filename, CPPCompilerParameters compilerparameters)
		{
			
			if (!CanCompile(filename))
			{
				MessageBox.Show("File " + filename + " is not a source file.", "Compilation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
				return new DefaultCompilerResult(new CompilerResults(new TempFileCollection()), "");
			}
			string output = "";
			string error  = "";
			string exe = Path.ChangeExtension(filename, ".exe");
			if (compilerparameters.OutputAssembly != null && compilerparameters.OutputAssembly.Length > 0) {
				exe = compilerparameters.OutputAssembly;
			}
			_treatWarningsAsErrors = compilerparameters.TreatWarningsAsErrors;
			string responseFileName = Path.GetTempFileName();
					
			StreamWriter writer = new StreamWriter(responseFileName);

			writer.WriteLine("/nologo");
			if (compilerparameters.UseManagedExtensions) 
			{
				writer.WriteLine("/clr");
			}
			writer.WriteLine("/Fe\"" + exe + "\"");
			
			writer.WriteLine('"' + filename + '"');
			
			TempFileCollection tf = new TempFileCollection();
			
			writer.Close();
			
			string compilerName = GetCompilerName();
			string outstr =  compilerName + " \"@" + responseFileName + "\"";
			string currDir = Directory.GetCurrentDirectory();
			string intDir = compilerparameters.IntermediateDirectory;
			if (intDir == null || intDir.Length == 0) {
				intDir = compilerparameters.OutputDirectory;
			}
			
			Directory.SetCurrentDirectory(intDir);
			ICompilerResult result;
			try {
				Executor.ExecWaitWithCapture(outstr, currDir, tf, ref output, ref error);
				result = ParseOutput(tf, output, error);
			}
			catch (System.Runtime.InteropServices.ExternalException e) {
				ShowErrorBox(e);
				result = CreateErrorCompilerResult(tf, e);
			}
			finally {
				File.Delete(responseFileName);
				File.Delete(output);
				File.Delete(error);
				Directory.SetCurrentDirectory(currDir);	
			}
			return result;
		}

		private void ShowErrorBox(System.Runtime.InteropServices.ExternalException e)
		{
			MessageBox.Show("It seems cl.exe is not installed or not found.\n\nInstall compiler and set PATH environment variable.\n\nException: " + e, "Compile Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
		
		public ICompilerResult CompileProject(IProject project, bool force)
		{
			_inputFiles   = new StringBuilder();
			_buildProcess = new StringBuilder();
			_results      = new StringBuilder();
					
			CPPProject p = (CPPProject)project;
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			_treatWarningsAsErrors = compilerparameters.TreatWarningsAsErrors;
			
			CheckDirectory(Path.GetDirectoryName(compilerparameters.OutputFile));
			CheckDirectory(Path.GetDirectoryName(compilerparameters.IntermediateDirectory));
			
			StringBuilder output = new StringBuilder();
			
			ICompilerResult result;
					
			if (compilerparameters.PreCompileHeader) {
				result = InternalCompileProject(p, true, force);
				if (result != null) {
					output.Append(result.CompilerOutput);
					if (HasErrors(result)) {
						goto exit;
					}
				}
			}
			
			result = InternalCompileProject(p, false, force);
			if (result != null) {
				output.Append(result.CompilerOutput);
				if (HasErrors(result)) {
					goto exit;
				}
			}
			if (result != null || !File.Exists(Path.GetFullPath(compilerparameters.OutputFile))) {
				result = LinkProject(p);
				output.Append(result.CompilerOutput);
			}
		exit:
			WriteResultFile(p);
			CompilerResults cr = result != null ? result.CompilerResults : new CompilerResults(new TempFileCollection());
			return new DefaultCompilerResult(cr, output.ToString());
		}
		
		private bool HasErrors(ICompilerResult compilerResult)
		{
			bool result = false;
			if (compilerResult.CompilerResults.Errors.Count > 0)
			{
				if (_treatWarningsAsErrors)
				{
					result = true;
				}
				else {
					foreach (CompilerError error in compilerResult.CompilerResults.Errors)
					{
						if (!error.IsWarning)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		private void CheckDirectory(string directory)
		{
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
		}
		
		private void WriteResultFile(CPPProject p)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			string directory  = Path.GetDirectoryName(compilerparameters.OutputFile);
			string resultFile = Path.Combine(directory, "BuildLog.html");
			_results.Append("writing result file to : " + resultFile);
			
			StreamWriter writer = new StreamWriter(resultFile);
			writer.Write("<HTML><HEAD></HEAD><BODY>");
			writer.Write("<TABLE WIDTH=100% BGCOLOR=LightBlue><TR><TD><FONT FACE=ARIAL SIZE=+3><B>Build Log from</B>: " + p.Name + "(" + compilerparameters.Name + ")</FONT></TABLE>");
			writer.Write("Build started.");
			writer.Write("<TABLE WIDTH=100% BGCOLOR=LightBlue><TR><TD><FONT FACE=ARIAL SIZE=+2>Command Line</FONT></TD></TR></TABLE>");
			writer.WriteLine(_inputFiles.ToString());
			
			writer.Write("<TABLE WIDTH=100% BGCOLOR=LightBlue><TR><TD><FONT FACE=ARIAL SIZE=+2>Output</FONT></TD></TR></TABLE>");
			writer.Write("<PRE>");
			writer.WriteLine(_buildProcess.ToString());
			writer.Write("</PRE>");
			
			writer.Write("<TABLE WIDTH=100% BGCOLOR=LightBlue><TR><TD><FONT FACE=ARIAL SIZE=+2>Results</FONT></TD></TR></TABLE>");
			writer.Write("<PRE>");
			writer.WriteLine(_results.ToString());
			writer.Write("</PRE>");
			writer.Write("Build finished.");
			writer.Write("</BODY></HTML>");
			writer.Close();
		}
		
		#region COMPILER
		private string GetCompilerName()
		{
			return @"cl.exe";
		}
		
		private string WriteCompilerParameters(CPPProject p, bool preCompiledHeader, bool force)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			StringBuilder sb = new StringBuilder();
			
			sb.Append("/c\n");
			
			if (compilerparameters.UseManagedExtensions) {
				sb.Append("/clr\n");
			}
			string directory = Path.GetDirectoryName(compilerparameters.OutputFile);
			sb.Append("/Fo\"");
			sb.Append(directory);
			sb.Append("/\"\n");
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(IProjectService));
			ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
			if (preCompiledHeader) {
				sb.Append(compilerparameters.GetPreCompiledHeaderOptions());
			} else {
				sb.Append(compilerparameters.GetCompilerOptions());
			}
			if (compilerparameters.AdditionalCompilerOptions != null && compilerparameters.AdditionalCompilerOptions.Length > 0) {
				foreach (string option in compilerparameters.AdditionalCompilerOptions.Split(';')) {
					sb.Append(option);
					sb.Append("\n");
				}
			}
			
			foreach (ProjectReference lib in p.ProjectReferences) {
				sb.Append("/FU\"");
				sb.Append(lib.GetReferencedFileName(p));
				sb.Append("\"\n");
			}
			
			switch (compilerparameters.ConfigurationType) {
				case ConfigurationType.Dll:
					sb.Append("/LD\n");
					break;
			}
			bool includedFile = false;
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							if (CanCompile(finfo.Name))
							{
								string fileName   = Path.GetFileNameWithoutExtension(Path.GetFullPath(finfo.Name)).ToLower();
								string headerFile = Path.GetFileNameWithoutExtension(compilerparameters.preCompiledHeaderCPPOptions.HeaderFile).ToLower();
								bool isPreHeader  = fileName == headerFile;
							
								if (!(preCompiledHeader ^ isPreHeader)) {
									if (force || ShouldCompileFile(p, finfo.Name)) {
										includedFile = true;
										sb.Append("\"");
										sb.Append(Path.GetFullPath(finfo.Name));
										sb.Append("\"\n");
									}
								}
							}
							break;
					}
				}
			}
			if (!includedFile) {
				return null;
			}
			string responseFileName = Path.GetTempFileName();
			StreamWriter writer = new StreamWriter(responseFileName, false);
//			string standardIncludes = Environment.GetEnvironmentVariable("INCLUDE");
//			if (standardIncludes != null && standardIncludes.Length > 0) {
//				writer.WriteLine("/I\"" + standardIncludes + "\"");
//			}
			
			writer.Write(sb.ToString());
			writer.Close();
			
			_inputFiles.Append("Creating temporary file ");
			_inputFiles.Append(responseFileName);
			_inputFiles.Append(" with following content:<BR>");
			_inputFiles.Append("<PRE>");
			_inputFiles.Append(sb.ToString());
			_inputFiles.Append("</PRE>");
			
			return responseFileName;
		}
		
		Hashtable lastCompiledFiles = new Hashtable();

		private bool ShouldCompileFile(CPPProject p, string fileName)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			string directory  = Path.GetDirectoryName(compilerparameters.OutputFile);
			string objectFile = Path.Combine(directory, Path.ChangeExtension(Path.GetFileName(fileName), ".obj"));
			if (!File.Exists(objectFile)) {
				return true;
			}
			
			string[] additinalIncludeDirs = compilerparameters.AdditionalCompilerOptions.Split(';');
			ArrayList dirs = new ArrayList(additinalIncludeDirs.Length+1);
			dirs.Add(Path.GetDirectoryName(fileName));
			foreach (string dir in additinalIncludeDirs)
			{
				dirs.Add(dir);
			}
			
			DateTime lastWriteTime = new IncludeParser(fileName, dirs, true).Parse().GetLastWriteTime();
//			DateTime lastWriteTime = File.GetLastWriteTime(fileName);
			
			bool shouldCompile;
			if (lastCompiledFiles[fileName] == null) {
				shouldCompile = true;
			} else {
				shouldCompile = lastWriteTime != (DateTime)lastCompiledFiles[fileName];
			}
			
			lastCompiledFiles[fileName] = lastWriteTime;
			return shouldCompile;
		}
		
		private ICompilerResult InternalCompileProject(CPPProject p, bool preCompiledHeader, bool force)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			
			string responseFileName = WriteCompilerParameters(p, preCompiledHeader, force);
			if (responseFileName == null) {
				return null;
			}
			string output = String.Empty;
			string error  = String.Empty; 
			
			string compilerName = GetCompilerName();
			string clstr = compilerName + " \"@" + responseFileName + "\"";
			
			TempFileCollection tf = new TempFileCollection();
			
			string currDir = Directory.GetCurrentDirectory();
			string intDir = compilerparameters.IntermediateDirectory;
			if (intDir == null || intDir.Length == 0)  {
				intDir = compilerparameters.OutputDirectory;
			}
			_inputFiles.Append("Executing command: <C>" + clstr + "</C><hr>");

			ICompilerResult result;
			try {
				Executor.ExecWaitWithCapture(clstr, tf, ref output, ref error);
				result = ParseOutput(tf, output, error);
			}
			catch (System.Runtime.InteropServices.ExternalException e) {
				ShowErrorBox(e);
				result = CreateErrorCompilerResult(tf, e);
			}
			finally {
				File.Delete(responseFileName);
				File.Delete(output);
				File.Delete(error);
			}
			
			return result;
		}
		#endregion

		#region LINKER
		private string GetLinkerName()
		{
			return @"link.exe";
		}
		
		private string WriteLinkerOptions(CPPProject p)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			
			StringBuilder sb = new StringBuilder();
			
			string exe = compilerparameters.OutputFile;
			string dir = Path.GetDirectoryName(Path.GetFullPath(exe));
			sb.Append("/OUT:\"");sb.Append(exe);sb.Append("\"\n");
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							if (CanCompile(finfo.Name))
							{
								sb.Append('"');
								sb.Append(Path.Combine(dir,
							    	                   Path.ChangeExtension(Path.GetFileName(finfo.Name), 
							        	               ".obj")));
								sb.Append("\"\n");
							}
							break;
						case BuildAction.EmbedAsResource:
							sb.Append("/ASSEMBLYRESOURCE:\"");sb.Append(Path.GetFullPath(finfo.Name));sb.Append("\"\n");
							break;
					}
				}
			}
			switch (compilerparameters.ConfigurationType) {
				case ConfigurationType.Dll:
					sb.Append("/DLL\n");
					break;
			}
			
			sb.Append(compilerparameters.GetLinkerOptionsForCompiler());
			
			// write to response file
			string responseFileName = Path.GetTempFileName();
			StreamWriter writer = new StreamWriter(responseFileName);
//			string standardLibs = Environment.GetEnvironmentVariable("LIB");
//			if (standardLibs != null && standardLibs.Length > 0) {
//				foreach (string lib in standardLibs.Split(';')) {
//					if (lib.Length > 0) {
//						writer.WriteLine("/LIBPATH:\"" + lib + "\"");
//					}
//				}
//			}
			writer.Write(sb.ToString());
			writer.Close();
			
			_inputFiles.Append("Creating temporary file <C>" + responseFileName + "</C> with following content:<BR>");
			_inputFiles.Append("<PRE>");
			_inputFiles.Append(sb.ToString());
			_inputFiles.Append("</PRE>");
			return responseFileName;
		}
		
		private ICompilerResult LinkProject(CPPProject p)
		{
			CPPCompilerParameters compilerparameters = (CPPCompilerParameters)p.ActiveConfiguration;
			
			string responseFileName = WriteLinkerOptions(p);
			
			string output = String.Empty;
			string error  = String.Empty; 
			
			string compilerName = GetLinkerName();
			string clstr = compilerName + " \"@" + responseFileName + "\"";
			
			TempFileCollection tf = new TempFileCollection();
			
			string currDir = Directory.GetCurrentDirectory();
			string intDir = compilerparameters.IntermediateDirectory;
			if (intDir == null || intDir.Length == 0)  {
				intDir = compilerparameters.OutputDirectory;
			}
			
			_inputFiles.Append("Executing command : <C>");
			_inputFiles.Append(clstr);
			_inputFiles.Append("</C><hr>");
			Executor.ExecWaitWithCapture(clstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output, error);
			
//			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			
			return result;
		}
		#endregion
		
		private ICompilerResult CreateErrorCompilerResult(TempFileCollection tf, System.Runtime.InteropServices.ExternalException e)
		{
			CompilerError error = new CompilerError();
			error.Line        = 0;
			error.FileName    = "";
			error.IsWarning   = false;
			error.ErrorNumber = "";
			error.ErrorText   = e.Message;
			CompilerResults cr = new CompilerResults(tf);
			cr.Errors.Add(error);
			return new DefaultCompilerResult(cr, "");
		}
		
		private void InternalParseOutputFile(StringBuilder compilerOutput, CompilerResults cr, string file)
		{
			StreamReader sr = new StreamReader(File.OpenRead(file), Encoding.Default);
			
			// skip fist whitespace line
			sr.ReadLine();
		
			
			while (true) {
				string curLine = sr.ReadLine();
				_buildProcess.Append(curLine);
				_buildProcess.Append("\n");
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
				curLine = curLine.Trim();
				if (curLine.Length == 0) {
					continue;
				}
				
				CompilerError error = new CompilerError();
				
				// try to match standard errors
				Match match = normalError.Match(curLine);
				if (match.Success) {
					error.Line = Int32.Parse(match.Result("${line}"));
					try {
						error.FileName = Path.GetFullPath(match.Result("${file}"));
					} catch (Exception) {
						error.FileName = "";
					}
					error.IsWarning   = match.Result("${error}").EndsWith("warning"); 
					error.ErrorNumber = match.Result("${number}");
					error.ErrorText   = match.Result("${message}");
				} else {
					match = generalError.Match(curLine); // try to match general csc errors
					if (match.Success) {
						error.IsWarning   = match.Result("${error}").EndsWith("warning");
						error.ErrorNumber = match.Result("${number}");
						error.ErrorText   = match.Result("${message}");
					} else { // give up and skip the line
						continue;
					}
				}
						
				cr.Errors.Add(error);
			}
			sr.Close();
		}
		
		private ICompilerResult ParseOutput(TempFileCollection tf, string outputFile, string errorFile)
		{
			StringBuilder compilerOutput = new StringBuilder();
			CompilerResults cr = new CompilerResults(tf);
			InternalParseOutputFile(compilerOutput, cr, outputFile);			
			InternalParseOutputFile(compilerOutput, cr, errorFile);			
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
		}
	}
}
