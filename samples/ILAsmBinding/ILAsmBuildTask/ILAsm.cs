// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ICSharpCode.Build.Tasks
{
	public sealed class ILAsm : ToolTask
	{
		ITaskItem outputAssembly;
		ITaskItem[] sources;
		string targetType;
		string keyContainer, keyFile;
		bool optimize;
		string debugType;
		ITaskItem[] resources;
		int fileAlignment;
		string emitDebugInformation;

		public string EmitDebugInformation {
			get {
				return emitDebugInformation;
			}
			set {
				emitDebugInformation = value;
			}
		}
		
		public int FileAlignment {
			get {
				return fileAlignment;
			}
			set {
				fileAlignment = value;
			}
		}
		
		public ITaskItem OutputAssembly {
			get {
				return outputAssembly;
			}
			set {
				outputAssembly = value;
			}
		}
		
		public ITaskItem[] Sources {
			get {
				return sources;
			}
			set {
				sources = value;
			}
		}
		
		public string TargetType {
			get {
				return targetType;
			}
			set {
				targetType = value;
			}
		}
		
		public string KeyContainer {
			get {
				return keyContainer;
			}
			set {
				keyContainer = value;
			}
		}
		
		public string KeyFile {
			get {
				return keyFile;
			}
			set {
				keyFile = value;
			}
		}
		
		public bool Optimize {
			get {
				return optimize;
			}
			set {
				optimize = value;
			}
		}
		
		public string DebugType {
			get {
				return debugType;
			}
			set {
				debugType = value;
			}
		}
		
		public ITaskItem[] Resources {
			get {
				return resources;
			}
			set {
				resources = value;
			}
		}
		
		protected override string ToolName {
			get {
				return "IlAsm.exe";
			}
		}
		
		protected override string GenerateFullPathToTool()
		{
			string path = ToolLocationHelper.GetPathToDotNetFrameworkFile(ToolName, TargetDotNetFrameworkVersion.VersionLatest);
			if (path == null) {
				base.Log.LogErrorWithCodeFromResources("General.FrameworksFileNotFound", ToolName, ToolLocationHelper.GetDotNetFrameworkVersionFolderPrefix(TargetDotNetFrameworkVersion.VersionLatest));
			}
			return path;
		}
		
		void AppendIntegerSwitch(CommandLineBuilder commandLine, string @switch, int value)
		{
			commandLine.AppendSwitchUnquotedIfNotNull(@switch, value.ToString(NumberFormatInfo.InvariantInfo));
		}
		
		protected override string GenerateCommandLineCommands()
		{
			CommandLineBuilder commandLine = new CommandLineBuilder();
			if (((OutputAssembly == null) && (Sources != null)) && ((Sources.Length > 0))) {
				OutputAssembly = new TaskItem(Path.GetFileNameWithoutExtension(this.Sources[0].ItemSpec));
				if (string.Equals(this.TargetType, "library", StringComparison.OrdinalIgnoreCase)) {
					OutputAssembly.ItemSpec += ".dll";
				} else if (string.Equals(this.TargetType, "module", StringComparison.OrdinalIgnoreCase)) {
					OutputAssembly.ItemSpec += ".netmodule";
				} else {
					OutputAssembly.ItemSpec += ".exe";
				}
			}
			commandLine.AppendSwitch("/NOLOGO");
			
			// TODO: EmitDebugInformation / DebugType
			commandLine.AppendSwitch("/DEBUG");
			
			if (optimize) {
				commandLine.AppendSwitch("/OPTIMIZE");
			}
			
			commandLine.AppendSwitchIfNotNull("/KEY=@", this.KeyContainer);
			commandLine.AppendSwitchIfNotNull("/KEY=", this.KeyFile);
			
			if (Resources != null) {
				foreach (ITaskItem item in Resources) {
					commandLine.AppendSwitchIfNotNull("/RESOURCE=", item);
				}
			}
			
			if (FileAlignment > 0) {
				AppendIntegerSwitch(commandLine, "/ALIGNMENT=", FileAlignment);
			}
			
			commandLine.AppendSwitchIfNotNull("/OUTPUT=", this.OutputAssembly);
			
			if (string.Equals(this.TargetType, "library", StringComparison.OrdinalIgnoreCase)) {
				commandLine.AppendSwitch("/DLL");
			} else if (string.Equals(this.TargetType, "module", StringComparison.OrdinalIgnoreCase)) {
				commandLine.AppendSwitch("/DLL");
			}
			
			commandLine.AppendFileNamesIfNotNull(this.Sources, " ");
			return commandLine.ToString();
		}
	}
}
