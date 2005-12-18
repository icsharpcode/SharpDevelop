// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using System;
using System.Text;
using System.IO;

namespace ICSharpCode.Build.Tasks
{
	public class CompilerCommandLineArguments : CommandLineBuilderExtension
	{		
		public CompilerCommandLineArguments(MonoCompilerTask compilerTask)
		{
			GenerateCommandLineArguments(compilerTask);
		}
		
		public static bool IsNetModule(string fileName)
		{
			return Path.GetExtension(fileName).ToLowerInvariant() == ".netmodule";
		}
		
		void GenerateCommandLineArguments(MonoCompilerTask compilerTask)
		{
			AppendSwitchIfTrue("-noconfig", compilerTask.NoConfig);				
			AppendSwitch("-warn:", compilerTask.WarningLevel.ToString());
			AppendFileNameIfNotNull("-out:", compilerTask.OutputAssembly);
			AppendTarget(compilerTask.TargetType);
			AppendSwitchWithoutParameterIfNotNull("-debug", compilerTask.DebugType);
			AppendSwitchIfTrue("-optimize", compilerTask.Optimize);			
			AppendSwitchIfTrue("-nologo", compilerTask.NoLogo);
			AppendSwitchIfTrue("-unsafe", compilerTask.AllowUnsafeBlocks);
			AppendSwitchIfTrue("-nostdlib", compilerTask.NoStandardLib);
			AppendSwitchIfTrue("-checked", compilerTask.CheckForOverflowUnderflow);
			AppendSwitchIfTrue("-delaysign", compilerTask.DelaySign);
			AppendSwitchIfNotNull("-langversion:", compilerTask.LangVersion);
			AppendSwitchIfNotNull("-keycontainer:", compilerTask.KeyContainer);
			AppendSwitchIfNotNull("-keyfile:", compilerTask.KeyFile);
			AppendSwitchIfNotNull("-define:", compilerTask.DefineConstants);
			AppendSwitchIfTrue("-warnaserror", compilerTask.TreatWarningsAsErrors);
			AppendSwitchIfNotNull("-nowarn:", compilerTask.DisabledWarnings);
			AppendSwitchIfNotNull("-main:", compilerTask.MainEntryPoint);
			AppendFileNameIfNotNull("-doc:", compilerTask.DocumentationFile);
			AppendSwitchIfNotNull("-lib:", compilerTask.AdditionalLibPaths, ",");
			AppendReferencesIfNotNull(compilerTask.References);
			AppendResourcesIfNotNull(compilerTask.Resources);
			AppendFileNameIfNotNull("-win32res:", compilerTask.Win32Resource);
			AppendFileNameIfNotNull("-win32icon:", compilerTask.Win32Icon);
			AppendFileNamesIfNotNull(compilerTask.Sources, " ");
		}
		
		void AppendReferencesIfNotNull(ITaskItem[] references)
		{
			if (references == null) {
				return;
			}
			
			foreach (ITaskItem reference in references) {
				string fileName = reference.ItemSpec;
				if (CompilerCommandLineArguments.IsNetModule(fileName)) {
					AppendFileNameIfNotNull("-addmodule:", reference);
				} else { 	
					AppendFileNameIfNotNull("-r:", reference);
				}
			}
		}
		
		void AppendResourcesIfNotNull(ITaskItem[] resources)
		{
			if (resources == null) {
				return;
			}
			
			foreach (ITaskItem resource in resources) {
				AppendFileNameIfNotNull("-resource:", resource);
			}
		}
		
		void AppendSwitchWithoutParameterIfNotNull(string switchName, string parameter)
		{
			if (parameter != null && parameter.Trim().Length > 0) {
				AppendSwitch(switchName);
			}
		}
		
		void AppendSwitchIfTrue(string switchName, bool parameter)
		{
			if (parameter) {
				AppendSwitch(switchName);
			}
		}
		
		void AppendSwitch(string switchName, string parameter)
		{
			AppendSwitchIfNotNull(switchName, parameter);
		}
		
		void AppendFileNameIfNotNull(string switchName, ITaskItem fileItem)
		{
			if (fileItem != null) {
				AppendFileNameIfNotNull(switchName, fileItem.ItemSpec);
			}
		}
		
		void AppendFileNameIfNotNull(string switchName, string fileName)
		{
			if (fileName != null) {
				AppendSpaceIfNotEmpty();
				AppendTextUnquoted(switchName);
				AppendFileNameWithQuoting(fileName);
			}
		}
		
		void AppendTarget(string targetType)
		{
			if (targetType != null) {
				AppendSwitch("-target:", targetType.ToLowerInvariant());
			}
		}
	}
}
