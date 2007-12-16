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

namespace Mono.Build.Tasks
{
	/// <summary>
	/// Base class for the Mcs and Gmcs tasks.
	/// </summary>
	public abstract class MonoCSharpCompilerTask : MonoCompilerTask
	{
		bool checkForOverflowUnderflow;
		bool delaySign;
		string documentationFile;
		string keyContainer;
		string keyFile;
		string langVersion;
		string moduleAssemblyName;
		bool noConfig;
		string win32Icon;
		string win32Resource;
		
		MonoCSharpCompilerResultsParser parser = new MonoCSharpCompilerResultsParser();
		
		public bool CheckForOverflowUnderflow {
			get { return checkForOverflowUnderflow; }
			set { checkForOverflowUnderflow = value; }
		}
		
		public bool DelaySign {
			get { return delaySign; }
			set { delaySign = value; }
		}
		
		public string DocumentationFile {
			get { return documentationFile; }
			set { documentationFile = value; }
		}
		
		public string KeyContainer {
			get { return keyContainer; }
			set { keyContainer = value; }
		}
		
		public string KeyFile {
			get { return keyFile; }
			set { keyFile = value; }
		}

		public string LangVersion {
			get { return langVersion; }
			set { langVersion = value; }
		}
		
		public string ModuleAssemblyName {
			get { return moduleAssemblyName; }
			set { moduleAssemblyName = value; }
		}

		public bool NoConfig {
			get { return noConfig; }
			set { noConfig = value; }
		}
		
		public string Win32Icon {
			get { return win32Icon; }
			set { win32Icon = value; }
		}
		
		public string Win32Resource {
			get { return win32Resource; }
			set { win32Resource = value; }
		}
		
		protected override string GenerateResponseFileCommands()
		{
			CompilerCommandLineArguments args = new CompilerCommandLineArguments();
			args.AppendSwitchIfTrue("-noconfig", noConfig);	
			if (IsWarningLevelSet) {
				args.AppendSwitch("-warn:", WarningLevel.ToString());
			}
			args.AppendFileNameIfNotNull("-out:", OutputAssembly);
			args.AppendTarget(TargetType);
			args.AppendSwitchIfTrue("-debug", EmitDebugInformation);
			args.AppendSwitchIfTrue("-optimize", Optimize);			
			args.AppendSwitchIfTrue("-nologo", NoLogo);
			args.AppendSwitchIfTrue("-unsafe", AllowUnsafeBlocks);
			args.AppendSwitchIfTrue("-nostdlib", NoStandardLib);
			args.AppendSwitchIfTrue("-checked", checkForOverflowUnderflow);
			args.AppendSwitchIfTrue("-delaysign", delaySign);
			args.AppendSwitchIfNotNull("-langversion:", langVersion);
			args.AppendSwitchIfNotNull("-keycontainer:", keyContainer);
			args.AppendSwitchIfNotNull("-keyfile:", keyFile);
			args.AppendSwitchIfNotNull("-define:", DefineConstants);
			args.AppendSwitchIfTrue("-warnaserror", TreatWarningsAsErrors);
			args.AppendSwitchIfNotNull("-nowarn:", DisabledWarnings);
			args.AppendSwitchIfNotNull("-main:", MainEntryPoint);
			args.AppendFileNameIfNotNull("-doc:", documentationFile);
			args.AppendSwitchIfNotNull("-lib:", AdditionalLibPaths, ",");
			args.AppendReferencesIfNotNull(References);
			args.AppendItemsIfNotNull("-resource:", Resources);
			args.AppendFileNameIfNotNull("-win32res:", win32Resource);
			args.AppendFileNameIfNotNull("-win32icon:", win32Icon);
			args.AppendFileNamesIfNotNull(Sources, " ");

			return args.ToString();
		}
		
		protected override CompilerError ParseLine(string line)
		{
			return parser.ParseLine(line);
		}
	}
}
