// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;

namespace FSharp.Build.Tasks
{
	public class CompilerCommandLineBuilder : CommandLineBuilder
	{
		public void AppendIntegerSwitch(string switchName, int value)
		{
			AppendSwitchUnquotedIfNotNull(switchName, value.ToString(NumberFormatInfo.InvariantInfo));
		}		
		
		public void AppendFileNameIfNotNull(string switchName, ITaskItem fileItem)
		{
			if (fileItem != null) {
				AppendFileNameIfNotNull(switchName, fileItem.ItemSpec);
			}
		}
		
		public void AppendFileNameIfNotNull(string switchName, string fileName)
		{
			if (fileName != null) {
				AppendSpaceIfNotEmpty();
				AppendTextUnquoted(switchName);
				AppendFileNameWithQuoting(fileName);
			}
		}			
	}
}
