// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;

namespace ICSharpCode.Build.Tasks
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
			get {
				return checkForOverflowUnderflow;
			}
			set {
				checkForOverflowUnderflow = value;
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
		
		public string DocumentationFile {
			get {
				return documentationFile;
			}
			set {
				documentationFile = value;
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
