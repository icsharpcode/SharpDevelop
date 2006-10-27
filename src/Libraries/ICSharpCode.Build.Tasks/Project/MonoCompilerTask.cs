// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using Microsoft.Build.Framework;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// Base class task for the mono compilers mcs, gmcs and mbas.
	/// </summary>
	public abstract class MonoCompilerTask : MyToolTask
	{		
		public const int DefaultWarningLevel = 2;
		
		string[] additionalLibPaths;
		string[] addModules;
		bool allowUnsafeBlocks;
		int codePage;
		string debugType;
		string defineConstants;
		string disabledWarnings;
		bool emitDebugInformation;
		ITaskItem[] linkResources;
		string mainEntryPoint;
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
		int? warningLevel;
		
		public bool AllowUnsafeBlocks {
			get {
				return allowUnsafeBlocks;
			}
			set {
				allowUnsafeBlocks = value;
			}
		}
		
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
		
		public string DisabledWarnings {
			get {
				return disabledWarnings;
			}
			set {
				disabledWarnings = value;
			}
		}

		public bool EmitDebugInformation {
			get {
				return emitDebugInformation;
			}
			set {
				emitDebugInformation = value;
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
				if (warningLevel.HasValue) {
					return warningLevel.Value;
				}
				return DefaultWarningLevel;
			}
			set {
				warningLevel = value;
			}
		}
		
		/// <summary>
		/// Determines whether the warning level property has been set.
		/// </summary>
		protected bool IsWarningLevelSet {
			get {
				return warningLevel.HasValue;
			}
		}
		
		/// <summary>
		/// Returns a compiler error from the standard output or standard
		/// error line.
		/// </summary>
		protected virtual CompilerError ParseLine(string line)
		{
			return null;
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
	}
}
