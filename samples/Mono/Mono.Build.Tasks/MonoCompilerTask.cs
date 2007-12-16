// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.CodeDom.Compiler;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Mono.Build.Tasks
{
	/// <summary>
	/// Base class task for the mono compilers mcs, gmcs and mbas.
	/// </summary>
	public abstract class MonoCompilerTask : ToolTask
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
			get { return allowUnsafeBlocks; }
			set { allowUnsafeBlocks = value; }
		}
		
		public string[] AdditionalLibPaths {
			get { return additionalLibPaths; }
			set { additionalLibPaths = value; }
		}

		public string[] AddModules {
			get { return addModules; }
			set { addModules = value; }
		}
		
		public int CodePage {
			get { return codePage; }
			set { codePage = value; }
		}
		
		public string DebugType {
			get { return debugType; }
			set { debugType = value; }
		}
		
		public string DefineConstants {
			get { return defineConstants; } 
			set { defineConstants = value; }
		}
		
		public string DisabledWarnings {
			get { return disabledWarnings; }
			set { disabledWarnings = value; }
		}

		public bool EmitDebugInformation {
			get { return emitDebugInformation; }
			set { emitDebugInformation = value; }
		}
		
		public ITaskItem[] LinkResources {
			get { return linkResources; }
			set { linkResources = value; }
		}
		
		public string MainEntryPoint {
			get { return mainEntryPoint; }
			set { mainEntryPoint = value; }
		}
	
		public bool NoLogo {
			get { return noLogo; }
			set { noLogo = value; }
		}
		
		public bool NoStandardLib {
			get { return noStandardLib; }
			set { noStandardLib = value; }
		}

		public bool Optimize {
			get { return optimize; }
			set { optimize = value; }
		}	
		
		public ITaskItem OutputAssembly {
			get { return outputAssembly; }
			set { outputAssembly = value; }
		}

		public ITaskItem[] References {
			get { return references; }
			set { references = value; }
		}
		
		public ITaskItem[] Resources {
			get { return resources; }
			set { resources = value; }
		}
		
		public ITaskItem[] ResponseFiles {
			get { return responseFiles; }
			set { responseFiles = value; }
		}

		public ITaskItem[] Sources {
			get { return sources; }
			set { sources = value; }
		}
		
		public string TargetType {
			get { return targetType; }
			set { targetType = value; }
		}
		
		public bool TreatWarningsAsErrors {
			get { return treatWarningsAsErrors; }
			set { treatWarningsAsErrors = value; }
		}
		
		public int WarningLevel {
			get {
				if (warningLevel.HasValue) {
					return warningLevel.Value;
				}
				return DefaultWarningLevel;
			}
			set { warningLevel = value; }
		}
		
		/// <summary>
		/// Determines whether the warning level property has been set.
		/// </summary>
		protected bool IsWarningLevelSet {
			get { return warningLevel.HasValue; }
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
