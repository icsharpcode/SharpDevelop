// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
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
