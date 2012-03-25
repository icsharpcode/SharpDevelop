// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class SvcUtilCommandLine
	{
		StringBuilder argumentBuilder = new StringBuilder();
		
		public SvcUtilCommandLine(SvcUtilOptions options)
		{
			GenerateCommandLine(options);
		}
		
		public string Command { get; set; }
		public string Arguments { get; private set; }
		
		void GenerateCommandLine(SvcUtilOptions options)
		{
			AppendIfNotEmpty("/out:", options.OutputFileName);
			AppendIfNotEmpty("/namespace:", options.GetNamespaceMapping());
			AppendIfNotEmpty("/language:", options.Language);
			AppendIfTrue("/noConfig", options.NoAppConfig);
			AppendIfTrue("/mergeConfig", options.MergeAppConfig);
			AppendIfNotEmpty("/config:", options.AppConfigFileName);
			AppendIfNotEmpty(options.Url);
			
			this.Arguments = argumentBuilder.ToString();
		}
		
		void AppendIfTrue(string argument, bool flag)
		{
			if (flag) {
				Append(argument);
			}
		}
		
		void AppendIfNotEmpty(string argumentName, string argumentValue)
		{
			if (!String.IsNullOrEmpty(argumentValue)) {
				Append(argumentName + GetQuotedArgument(argumentValue));
			}
		}
		
		void AppendIfNotEmpty(string argument)
		{
			if (!String.IsNullOrEmpty(argument)) {
				Append(argument);
			}
		}
		
		void Append(string argument)
		{
			argumentBuilder.Append(argument);
			argumentBuilder.Append(' ');
		}
		
		public override string ToString()
		{
			return String.Format(
				"{0} {1}",
				GetQuotedCommand(),
				Arguments);
		}
		
		string GetQuotedCommand()
		{
			return GetQuotedArgument(Command);
		}
		
		string GetQuotedArgument(string argument)
		{
			if (ContainsSpaceCharacter(argument)) {
				return String.Format("\"{0}\"", argument);
			}
			return argument;
		}
		
		bool ContainsSpaceCharacter(string text)
		{
			return text.IndexOf(' ') >= 0;
		}
	}
}
