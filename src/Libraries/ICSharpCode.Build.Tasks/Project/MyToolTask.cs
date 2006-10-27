// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using Microsoft.Build.Utilities;

namespace ICSharpCode.Build.Tasks
{
	public abstract class MyToolTask : ToolTask
	{
		protected override string GenerateFullPathToTool()
		{
			string path = ToolLocationHelper.GetPathToDotNetFrameworkFile(ToolName, Constants.DefaultFramework);
			if (path == null) {
				base.Log.LogErrorWithCodeFromResources("General.FrameworksFileNotFound", ToolName, ToolLocationHelper.GetDotNetFrameworkVersionFolderPrefix(Constants.DefaultFramework));
			}
			return path;
		}
		
		protected void AppendIntegerSwitch(CommandLineBuilder commandLine, string @switch, int value)
		{
			commandLine.AppendSwitchUnquotedIfNotNull(@switch, value.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}
