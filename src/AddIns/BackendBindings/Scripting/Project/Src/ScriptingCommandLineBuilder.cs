// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.Scripting
{
	public class ScriptingCommandLineBuilder
	{
		StringBuilder commandLine = new StringBuilder();
		
		public ScriptingCommandLineBuilder()
		{
		}
		
		public override string ToString()
		{
			return commandLine.ToString().TrimEnd();
		}
		
		public void AppendBooleanOptionIfTrue(string option, bool flag)
		{
			if (flag) {
				AppendOption(option);
			}
		}
		
		public void AppendOption(string option)
		{
			commandLine.Append(option + " ");
		}
		
		public void AppendQuotedStringIfNotEmpty(string option)
		{
			if (!String.IsNullOrEmpty(option)) {
				AppendQuotedString(option);
			}
		}
		
		public void AppendQuotedString(string option)
		{
			string quotedOption = String.Format("\"{0}\"", option);
			AppendOption(quotedOption);
		}
		
		public void AppendStringIfNotEmpty(string option)
		{
			if (!String.IsNullOrEmpty(option)) {
				AppendOption(option);
			}
		}
	}
}
