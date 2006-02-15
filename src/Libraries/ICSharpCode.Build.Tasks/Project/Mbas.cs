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
	/// MSBuild task for Mono's Visual Basic compiler Mbas.
	/// </summary>
	public class Mbas : MonoCompilerTask
	{
		ITaskItem[] imports;
		string optionCompare;
		bool optionExplicit;
		bool optionStrict;
		bool noWarnings;
		bool removeIntegerChecks;
		string rootNamespace;
		
		public ITaskItem[] Imports {
			get {
				return imports;
			}
			set {
				imports = value;
			}
		}
		
		public string OptionCompare {
			get {
				return optionCompare;
			}
			set {
				optionCompare = value;
			}
		}
		
		public bool OptionExplicit {
			get {
				return optionExplicit;
			}
			set {
				optionExplicit = value;
			}
		}
		
		public bool OptionStrict {
			get {
				return optionStrict;
			}
			set {
				optionStrict = value;
			}
		}
		
		public bool NoWarnings {
			get {
				return noWarnings;
			}
			set {
				noWarnings = value;
			}
		}
		
		public bool RemoveIntegerChecks {
			get {
				return removeIntegerChecks;
			}
			set {
				removeIntegerChecks = value;
			}
		}
		
		public string RootNamespace {
			get {
				return rootNamespace;
			}
			set {
				rootNamespace = value;
			}
		}
		
		/// <summary>
		/// Command line arguments that will be passed to the compiler.
		/// </summary>
		protected override string GenerateCommandLineArguments()
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
		
		protected override ICompilerResultsParser GetCompilerResultsParser()
		{
			return new MonoBasicCompilerResultsParser();
		}
		
		protected override string ToolName {
			get {
				return "Mbas.exe";
			}
		}
		
		protected override string GenerateFullPathToTool()
		{
			return MonoToolLocationHelper.GetPathToTool(ToolName);
		}
	}
}
