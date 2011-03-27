// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

using ICSharpCode.Scripting;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellHostUserInterface : PSHostUserInterface
	{
		IScriptingConsole scriptingConsole;
		PowerShellHostRawUserInterface rawUI;
		
		public PowerShellHostUserInterface(IScriptingConsole scriptingConsole)
		{
			this.scriptingConsole = scriptingConsole;
			rawUI = new PowerShellHostRawUserInterface(scriptingConsole);
		}
		
		public override void WriteWarningLine(string message)
		{
			scriptingConsole.WriteLine(message, ScriptingStyle.Warning);
		}
		
		public override void WriteVerboseLine(string message)
		{
			scriptingConsole.WriteLine(message, ScriptingStyle.Out);
		}
		
		public override void WriteProgress(long sourceId, ProgressRecord record)
		{
		}
		
		public override void WriteLine(string value)
		{
			scriptingConsole.WriteLine(value, ScriptingStyle.Out);
		}
		
		public override void WriteErrorLine(string value)
		{
			scriptingConsole.WriteLine(value, ScriptingStyle.Error);
		}
		
		public override void WriteDebugLine(string message)
		{
			scriptingConsole.WriteLine(message, ScriptingStyle.Out);
		}
		
		public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
		{
			scriptingConsole.Write(value, ScriptingStyle.Out);
		}
		
		public override void Write(string value)
		{
			scriptingConsole.Write(value, ScriptingStyle.Out);
		}
		
		public override SecureString ReadLineAsSecureString()
		{
			return null;
		}
		
		public override string ReadLine()
		{
			return null;
		}
		
		public override PSHostRawUserInterface RawUI {
			get { return rawUI; }
		}
		
		public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, System.Management.Automation.PSCredentialUIOptions options)
		{
			return null;
		}
		
		public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
		{
			return null;
		}
		
		public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
		{
			// No choice.
			return -1;
		}
		
		public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
		{
			return null;
		}
	}
}
