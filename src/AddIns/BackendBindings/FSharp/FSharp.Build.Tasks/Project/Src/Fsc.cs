// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Robert Pickering" email="robert@strangelights.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Globalization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FSharp.Build.Tasks 
{
	public sealed class Fsc : ToolTask 
	{
		string keyContainer;
		string keyFile;
		int fileAlignment;
		bool standalone;
		bool noMLLib;
		ITaskItem[] sources;
		ITaskItem[] resources;
		ITaskItem[] references;
		ITaskItem outputAssembly;
		string debugType;
		bool emitDebugInformation;
		string targetType;
		bool optimize;
		
		public int FileAlignment {
		  get { return fileAlignment; }
		  set { fileAlignment = value; }
		}
		
		public string KeyContainer {
		  get { return keyContainer; }
		  set { keyContainer = value; }
		}
	
		public string KeyFile {
		  get { return keyFile; }
		  set { keyFile = value; }
		}
	
		public bool Standalone {
		  get { return standalone; }
		  set { standalone = value; }
		}
	
		public bool NoMLLib {
		  get { return noMLLib; }
		  set { noMLLib = value; }
		}
		
		public string DebugType {
			get { return debugType; }
			set { debugType = value; }
		}		

		public bool EmitDebugInformation {
			get { return emitDebugInformation; }
			set { emitDebugInformation = value; }
		}
		
		public ITaskItem[] Sources {
			get { return sources; }
			set { sources = value; }
		}
		
		public ITaskItem[] References {
			get { return references; }
			set { references = value; }
		}		

		public ITaskItem[] Resources {
			get { return resources; }
			set { resources = value; }
		}		
		
		public bool Optimize {
			get { return optimize; }
			set { optimize = value; }
		}			
		
		public ITaskItem OutputAssembly {
			get { return outputAssembly; }
			set { outputAssembly = value; }
		}		
		
		public string TargetType {
			get { return targetType; }
			set { targetType = value; }
		}		
					
		protected override string ToolName {
		  get { return "fsc.exe"; }
		}

		protected override string GenerateFullPathToTool() 
		{
			return FscToolLocationHelper.GetPathToTool();
		}
	
		protected override string GenerateCommandLineCommands() 
		{
			CompilerCommandLineBuilder commandLine = new CompilerCommandLineBuilder();
			if (((OutputAssembly == null) && (Sources != null)) && ((Sources.Length > 0))) {
				OutputAssembly = new TaskItem(Path.GetFileNameWithoutExtension(this.Sources[0].ItemSpec));
				if (string.Equals(this.TargetType, "library", StringComparison.InvariantCultureIgnoreCase)) {
					OutputAssembly.ItemSpec += ".dll";
				} else if (string.Equals(this.TargetType, "module", StringComparison.InvariantCultureIgnoreCase)) {
					OutputAssembly.ItemSpec += ".netmodule";
				} else {
					OutputAssembly.ItemSpec += ".exe";
				}
			}
	
			commandLine.AppendSwitch("-g");
			
			if (Optimize) {
				commandLine.AppendSwitch("-O3");
			}
			
			commandLine.AppendSwitchIfNotNull("--keyfile ", this.KeyFile);
			
			if (noMLLib) {
				commandLine.AppendSwitch("--no-mllib");
			}
			
			if (standalone) {
				commandLine.AppendSwitch("--standalone");
			}
			
			if (Resources != null) {
				foreach (ITaskItem item in Resources) {
					commandLine.AppendSwitchIfNotNull("--resource ", item);
				}
			}
			
			if (FileAlignment > 0) {
				commandLine.AppendIntegerSwitch("--base-address ", FileAlignment);
			}
			
			commandLine.AppendSwitchIfNotNull("-o ", this.OutputAssembly);
			
			if (string.Equals(this.TargetType, "library", StringComparison.InvariantCultureIgnoreCase)) {
				commandLine.AppendSwitch("--target-dll");
			} else if (string.Equals(this.TargetType, "winexe", StringComparison.InvariantCultureIgnoreCase)) {
				commandLine.AppendSwitch("--target-winexe");
			} else if (string.Equals(this.TargetType, "module", StringComparison.InvariantCultureIgnoreCase)) {
				commandLine.AppendSwitch("--target-module");
			}
			
			if (References != null) {
				foreach (ITaskItem reference in References) {
					commandLine.AppendFileNameIfNotNull("-r ", reference);
				}
			}
			commandLine.AppendFileNamesIfNotNull(this.Sources, " ");
			return commandLine.ToString();
		}
			
		/// <summary>
		/// Each line of output, from either standard output or standard error
		/// is parsed and any compiler errors are logged.  If the line cannot
		/// be parsed it is logged using the specified message importance.
		/// </summary>
		protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
		{
			CompilerError error = ParseLine(singleLine);
			if (error != null) {
				if (error.IsWarning) {
					Log.LogWarning("warning", error.ErrorNumber, null, error.FileName, error.Line, error.Column, error.Line, error.Column, error.ErrorText);
				} else {
					Log.LogError("error", error.ErrorNumber, null, error.FileName, error.Line, error.Column, error.Line, error.Column, error.ErrorText);
				}
			} else {
				Log.LogMessage(messageImportance, singleLine);
			}
		}	
		
	
		const string ErrorPattern = @"(?<file>[^\(]*)\((?<line>[0-9]*),(?<column>[0-9]*)\):\s(?<error>[^:]*):\s(?<number>[^:]*):\s(?<message>.*)";
		static Regex ErrorRegex = new Regex(ErrorPattern, RegexOptions.Compiled);
		const string ErrorPatternNoLine = @"(?<file>[^\(]*)\((?<line>[0-9]*),(?<column>[0-9]*)\):\s(?<error>[^:]*):\s(?<message>.*)";
		static Regex ErrorNoLineRegex = new Regex(ErrorPatternNoLine, RegexOptions.Compiled);
	
		CompilerError ParseLine(string line) 
		{
			Match match = ErrorRegex.Match(line);
			Match matchNoLine = ErrorNoLineRegex.Match(line);
			if (match.Success) {
				CompilerError error = new CompilerError();
				error.Column = Int32.Parse(match.Result("${column}"));
				error.Line = Int32.Parse(match.Result("${line}"));
				error.FileName = Path.GetFullPath(match.Result("${file}"));
				error.IsWarning = match.Result("${error}") == "warning";
				error.ErrorNumber = match.Result("${number}");
				error.ErrorText = match.Result("${message}");
				return error;
			} else if (matchNoLine.Success) {
				CompilerError error = new CompilerError();
				error.Column = Int32.Parse(matchNoLine.Result("${column}"));
        error.Line = Int32.Parse(matchNoLine.Result("${line}"));
				error.FileName = Path.GetFullPath(matchNoLine.Result("${file}"));
				error.IsWarning = matchNoLine.Result("${error}") == "warning";
				error.ErrorNumber = matchNoLine.Result("${number}");
				error.ErrorText = matchNoLine.Result("${message}");
				return error;
			}
			return null;
		}	
	}
}
