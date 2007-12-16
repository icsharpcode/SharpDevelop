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

namespace Mono.Build.Tasks
{
	/// <summary>
	/// MSBuild task for Mono's Visual Basic compiler Vbnc.
	/// </summary>
	public class Vbnc : MonoCompilerTask
	{
		ITaskItem[] imports;
		string optionCompare;
		bool optionExplicit;
		bool optionStrict;
		bool noWarnings;
		bool removeIntegerChecks;
		string rootNamespace;
		
		MonoBasicCompilerResultsParser parser = new MonoBasicCompilerResultsParser();
		
		public ITaskItem[] Imports {
			get { return imports; }
			set { imports = value; } 
		}
		
		public string OptionCompare {
			get { return optionCompare; }
			set { optionCompare = value; }
		}
		
		public bool OptionExplicit {
			get { return optionExplicit; }
			set { optionExplicit = value; }
		}
		
		public bool OptionStrict {
			get { return optionStrict; }
			set { optionStrict = value; }
		}
		
		public bool NoWarnings {
			get { return noWarnings; }
			set { noWarnings = value; }
		}
		
		public bool RemoveIntegerChecks {
			get { return removeIntegerChecks; }
			set { removeIntegerChecks = value; }
		}
		
		public string RootNamespace {
			get { return rootNamespace; }
			set { rootNamespace = value; }
		}
		
		protected override string GenerateResponseFileCommands()
		{
			CompilerCommandLineArguments args = new CompilerCommandLineArguments();
						
			args.AppendFileNameIfNotNull("-out:", OutputAssembly);
			if (IsWarningLevelSet) {
				args.AppendSwitch("-wlevel:", WarningLevel.ToString());
			}
			args.AppendTarget(TargetType);
			args.AppendSwitchIfTrue("-debug", EmitDebugInformation);
			args.AppendLowerCaseSwitchIfNotNull("-debug:", DebugType);
			args.AppendSwitchIfTrue("-nologo", NoLogo);
			args.AppendSwitchIfTrue("-nowarn", noWarnings);
			args.AppendSwitchIfTrue("-unsafe", AllowUnsafeBlocks);
			args.AppendSwitchIfTrue("-nostdlib", NoStandardLib);
			args.AppendSwitchIfNotNull("-define:", DefineConstants);
			args.AppendSwitchIfNotNull("-main:", MainEntryPoint);
			args.AppendSwitchIfNotNull("-lib:", AdditionalLibPaths, ",");
			args.AppendSwitchIfNotNull("-ignorewarn:", DisabledWarnings);
			args.AppendSwitchIfTrue("-optionstrict", OptionStrict);
			args.AppendSwitchIfTrue("-optionexplicit", OptionExplicit);
			args.AppendSwitchIfTrue("-warnaserror", TreatWarningsAsErrors);
			args.AppendSwitchIfTrue("-removeintchecks", removeIntegerChecks);
			args.AppendSwitchIfNotNull("-rootnamespace:", rootNamespace);
			args.AppendItemsIfNotNull("-imports:", Imports);
			args.AppendReferencesIfNotNull(References);
			args.AppendItemsIfNotNull("-resource:", Resources);
			args.AppendFileNamesIfNotNull(Sources, " ");

			return args.ToString();
		}
			
		protected override string ToolName {
			get { return "Vbnc.exe"; }
		}
		
		protected override string GenerateFullPathToTool()
		{
			return MonoToolLocationHelper.GetPathToTool(ToolName);
		}
		
		protected override CompilerError ParseLine(string line)
		{
			return parser.ParseLine(line);
		}
	}
}
