// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.CodeDom.Compiler;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace VBBinding {
	
	/// <summary>
	/// This class controls the compilation of VB.net files and VB.net projects
	/// </summary>
	public class VBBindingCompilerServices
	{	
		
		PropertyService PropertyService       = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			VBProject p = (VBProject)project;
			VBCompilerParameters compilerparameters = (VBCompilerParameters)p.ActiveConfiguration;
			return FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName) == ".vb";
		}
		
		string GetCompilerName(string compilerVersion)
		{
			string runtimeDirectory = Path.Combine(FileUtility.NETFrameworkInstallRoot, compilerVersion);
			if (compilerVersion.Length == 0 || compilerVersion == "Standard" || !Directory.Exists(runtimeDirectory)) {
				runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			}
			return '"' + Path.Combine(runtimeDirectory, "vbc.exe") + '"';
		}
		
		string GenerateOptions(VBCompilerParameters compilerparameters, string outputFileName)
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append("\"/out:");sb.Append(outputFileName);sb.Append('"');sb.Append(Environment.NewLine);
			
			sb.Append("/nologo");sb.Append(Environment.NewLine);
			sb.Append("/utf8output");sb.Append(Environment.NewLine);
			
			if (compilerparameters.Debugmode) {
				sb.Append("/debug+");sb.Append(Environment.NewLine);
				sb.Append("/debug:full");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.Optimize) {
				sb.Append("/optimize");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.OptionStrict) {
				sb.Append("/optionstrict+");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.OptionExplicit) {
				sb.Append("/optionexplicit+");sb.Append(Environment.NewLine);
			} else {
				sb.Append("/optionexplicit-");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.Win32Icon != null && compilerparameters.Win32Icon.Length > 0 && File.Exists(compilerparameters.Win32Icon)) {
				sb.Append("/win32icon:");sb.Append('"');sb.Append(compilerparameters.Win32Icon);sb.Append('"');sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.RootNamespace!= null && compilerparameters.RootNamespace.Length > 0) {
				sb.Append("/rootnamespace:");sb.Append('"');sb.Append(compilerparameters.RootNamespace);sb.Append('"');sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.DefineSymbols.Length > 0) {
				sb.Append("/define:");sb.Append('"');sb.Append(compilerparameters.DefineSymbols);sb.Append('"');sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.MainClass != null && compilerparameters.MainClass.Length > 0) {
				sb.Append("/main:");sb.Append(compilerparameters.MainClass);sb.Append(Environment.NewLine);
			}
			
			if(compilerparameters.Imports.Length > 0) {
				sb.Append("/imports:");sb.Append(compilerparameters.Imports);sb.Append(Environment.NewLine);
			}
			
			switch (compilerparameters.CompileTarget) {
				case CompileTarget.Exe:
					sb.Append("/target:exe");
					break;
				case CompileTarget.WinExe:
					sb.Append("/target:winexe");
					break;
				case CompileTarget.Library:
					sb.Append("/target:library");
					break;
				case CompileTarget.Module:
					sb.Append("/target:module");
					break;
				default:
					throw new NotSupportedException("unknwon compile target:" + compilerparameters.CompileTarget);
			}
			sb.Append(Environment.NewLine);
			return sb.ToString();
		}
		
		public ICompilerResult CompileFile(string filename)
		{
			string output = "";
			string error  = "";
			string exe = Path.ChangeExtension(filename, ".exe");
			VBCompilerParameters compilerparameters = new VBCompilerParameters();
			string stdResponseFileName = PropertyService.DataDirectory + Path.DirectorySeparatorChar + "vb.rsp";
			
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);
			writer.WriteLine(GenerateOptions(compilerparameters, exe));
			writer.WriteLine('"' + filename + '"');
			writer.Close();
			
			string compilerName = GetCompilerName(compilerparameters.VBCompilerVersion);
			string outstr = compilerName + " \"@" + responseFileName + "\" \"@" + stdResponseFileName + "\"";
			
			TempFileCollection  tf = new TempFileCollection ();
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			WriteManifestFile(exe);
			return result;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			VBProject p = (VBProject)project;
			VBCompilerParameters compilerparameters = (VBCompilerParameters)p.ActiveConfiguration;
			string exe       = FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			string responseFileName = Path.GetTempFileName();
			string stdResponseFileName = PropertyService.DataDirectory + Path.DirectorySeparatorChar + "vb.rsp";
			
			foreach (ProjectFile finfo in project.ProjectFiles) {
				if (Path.GetFileName(finfo.Name).ToLower() == "app.config") {
					try {
						File.Copy(finfo.Name, exe + ".config", true);
					} catch (Exception ex) {
						
						MessageService.ShowError(ex);
					}
					break;
				}
			}
			
			StreamWriter writer = new StreamWriter(responseFileName);
			writer.WriteLine(GenerateOptions(compilerparameters, exe));
			
			foreach (ProjectReference lib in p.ProjectReferences) {
				string fileName = lib.GetReferencedFileName(p);
				writer.WriteLine("\"/r:" + fileName + "\"");
			}
			
			// write source files and embedded resources
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							writer.WriteLine('"' + finfo.Name + '"');
						break;
						
						case BuildAction.EmbedAsResource:
							writer.WriteLine("\"/res:" + finfo.Name + "\"");
						break;
					}
				}
			}
			
			TempFileCollection tf = new TempFileCollection ();
			writer.Close();
			
			string output = "";
			string error  = "";
			string compilerName = GetCompilerName(compilerparameters.VBCompilerVersion);
			string outstr = compilerName + " \"@" + responseFileName + "\" \"@" + stdResponseFileName + "\"";
			
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			ICompilerResult result = ParseOutput(tf, output);
			project.CopyReferencesToOutputPath(false);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			if (compilerparameters.CompileTarget != CompileTarget.Library) {
				WriteManifestFile(exe);
			}
			return result;
		}
		
		// code duplication: see C# backend : CSharpBindingCompilerManager
		void WriteManifestFile(string fileName)
		{
			string manifestFile = fileName + ".manifest";
			if (File.Exists(manifestFile)) {
				return;
			}
			StreamWriter sw = new StreamWriter(manifestFile);
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
			sw.WriteLine("");
			sw.WriteLine("<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\">");
			sw.WriteLine("	<dependency>");
			sw.WriteLine("		<dependentAssembly>");
			sw.WriteLine("			<assemblyIdentity");
			sw.WriteLine("				type=\"win32\"");
			sw.WriteLine("				name=\"Microsoft.Windows.Common-Controls\"");
			sw.WriteLine("				version=\"6.0.0.0\"");
			sw.WriteLine("				processorArchitecture=\"X86\"");
			sw.WriteLine("				publicKeyToken=\"6595b64144ccf1df\"");
			sw.WriteLine("				language=\"*\"");
			sw.WriteLine("			/>");
			sw.WriteLine("		</dependentAssembly>");
			sw.WriteLine("	</dependency>");
			sw.WriteLine("</assembly>");
			sw.Close();
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			StringBuilder compilerOutput = new StringBuilder();
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
			while (true) {
				string next = sr.ReadLine();
				compilerOutput.Append(next);compilerOutput.Append(Environment.NewLine);
				if (next == null) {
					break;
				}
				CompilerError error = new CompilerError();
				
				int index           = next.IndexOf(": ");
				if (index < 0) {
					continue;
				}
				
				string description  = null;
				string errorwarning = null;
				string location     = null;
				
				string s1 = next.Substring(0, index);
				string s2 = next.Substring(index + 2);
				index  = s2.IndexOf(": ");
				
				if (index == -1) {
					errorwarning = s1;
					description = s2;
				} else {
					location = s1;
					s1 = s2.Substring(0, index);
					s2 = s2.Substring(index + 2);
					errorwarning = s1;
					description = s2;
				}
				
				if (location != null) {
					int idx1 = location.LastIndexOf('(');
					int idx2 = location.LastIndexOf(')');
					if (idx1 >= 0 &&  idx2 >= 0) {
						string filename = location.Substring(0, idx1);
						error.Line = Int32.Parse(location.Substring(idx1 + 1, idx2 - idx1 - 1));
						error.FileName = Path.GetFullPath(filename.Trim()); // + "\\" + Path.GetFileName(filename);
					}
				}
				
				string[] what = errorwarning.Split(' ');
				error.IsWarning   = what[0] == "warning";
				error.ErrorNumber = what[what.Length - 1];
				
				error.ErrorText = description;
				
				cr.Errors.Add(error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
		}
	}
}
