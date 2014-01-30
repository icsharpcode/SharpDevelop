// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
