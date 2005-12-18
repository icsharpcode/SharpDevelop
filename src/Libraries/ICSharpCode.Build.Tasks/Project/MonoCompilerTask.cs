// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.CodeDom.Compiler;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// Base class task for the mono compilers mcs and gmcs.
	/// </summary>
	public abstract class MonoCompilerTask : MyToolTask
	{		
		string[] additionalLibPaths;
		string[] addModules;
		bool allowUnsafeBlocks;
		bool checkForOverflowUnderflow;
		int codePage;
		string debugType;
		string defineConstants;
		bool delaySign;
		string disabledWarnings;
		string documentationFile;
		string emitDebugInformation;
		string keyContainer;
		string keyFile;
		string langVersion;
		ITaskItem[] linkResources;
		string mainEntryPoint;
		string moduleAssemblyName;
		bool noConfig;
		bool noLogo;
		bool noStandardLib;
		bool optimize;
		ITaskItem outputAssembly;
		ITaskItem[] references;
		ITaskItem[] resources;
		ITaskItem[] responseFiles;
		ITaskItem[] sources;
		string targetType;
		bool treatWarningsAsErrors;
		int warningLevel;
		string win32Icon;
		string win32Resource;
		
		public string[] AdditionalLibPaths {
			get {
				return additionalLibPaths;
			}
			set {
				additionalLibPaths = value;
			}
		}

		public string[] AddModules {
			get {
				return addModules;
			}
			set {
				addModules = value;
			}
		}
		
		public bool AllowUnsafeBlocks {
			get {
				return allowUnsafeBlocks;
			}
			set {
				allowUnsafeBlocks = value;
			}
		}
		
		public bool CheckForOverflowUnderflow {
			get {
				return checkForOverflowUnderflow;
			}
			set {
				checkForOverflowUnderflow = value;
			}
		}
		
		public int CodePage {
			get {
				return codePage;
			}
			set {
				codePage = value;
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

		public string DefineConstants {
			get {
				return defineConstants;
			} 
			set {
				defineConstants = value;
			}
		}
		
		public bool DelaySign {
			get {
				return delaySign;
			}
			
			set {
				delaySign = value;
			}
		}

		public string DisabledWarnings {
			get {
				return disabledWarnings;
			}
			set {
				disabledWarnings = value;
			}
		}
		
		public string DocumentationFile {
			get {
				return documentationFile;
			}
			set {
				documentationFile = value;
			}
		}
		
		public string EmitDebugInformation {
			get {
				return emitDebugInformation;
			}
			set {
				emitDebugInformation = value;
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

		public string LangVersion {
			get {
				return langVersion; 
			}
			set {
				langVersion = value;
			}
		}

		public ITaskItem[] LinkResources {
			get {
				return linkResources;
			}
			set {
				linkResources = value;
			}
		}
		
		public string MainEntryPoint {
			get {
				return mainEntryPoint;
			}
			set {
				mainEntryPoint = value;
			}
		}
		
		public string ModuleAssemblyName {
			get {
				return moduleAssemblyName;
			}
			set {
				moduleAssemblyName = value;
			}
		}

		public bool NoConfig {
			get {
				return noConfig;
			}
			set {
				noConfig = value;
			}
		}

		public bool NoLogo {
			get {
				return noLogo;
			}
			set {
				noLogo = value;
			}
		}

		public bool NoStandardLib {
			get {
				return noStandardLib;
			}
			set {
				noStandardLib = value;
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
		public ITaskItem OutputAssembly {
			get {
				return outputAssembly;
			}
			set {
				outputAssembly = value;
			}
		}

		public ITaskItem[] References {
			get {
				return references;
			}
			set {
				references = value;
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
		
		public ITaskItem[] ResponseFiles {
			get {
				return responseFiles;
			}
			set {
				responseFiles = value;
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
		
		public bool TreatWarningsAsErrors {
			get {
				return treatWarningsAsErrors;
			}
			set {
				treatWarningsAsErrors = value;
			}
		}
		
		public int WarningLevel {
			get {
				return warningLevel;
			}
			set {
				warningLevel = value;
			}
		}
		
		public string Win32Icon {
			get {
				return win32Icon;
			}
			set {
				win32Icon = value;
			}
		}
		
		public string Win32Resource {
			get {
				return win32Resource;
			}
			set {
				win32Resource = value;
			}
		}

		public override bool Execute()
		{
			string args = GenerateCommandLineArguments();
			
			ToolPath = GenerateFullPathToTool();
			MonoCompiler compiler = new MonoCompiler();
			int returnValue = compiler.Run(ToolPath, args, GetCompilerResultsParser());

			int errorCount = 0;
			foreach (CompilerError error in compiler.Results.Errors) {
				if (error.IsWarning) {
					Log.LogWarning("warning", error.ErrorNumber, null, error.FileName, error.Line, error.Column, error.Line, error.Column, error.ErrorText);
				} else {
					errorCount++;
					Log.LogError("error", error.ErrorNumber, null, error.FileName, error.Line, error.Column, error.Line, error.Column, error.ErrorText);
				}
			}
			
			if (returnValue != 0 && errorCount == 0) {
				Log.LogError(String.Concat("Failed to execute compiler: ", returnValue, ": ", ToolPath));
			}
			
			return errorCount == 0;
		}
		
		/// <summary>
		/// Command line arguments that will be passed to the compiler.
		/// </summary>
		protected virtual string GenerateCommandLineArguments()
		{
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(this);
			return args.ToString();
		}
		
		protected virtual ICompilerResultsParser GetCompilerResultsParser()
		{
			return new CompilerResultsParser();
		}
	}
}
