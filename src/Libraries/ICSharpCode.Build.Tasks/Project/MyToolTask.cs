// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
