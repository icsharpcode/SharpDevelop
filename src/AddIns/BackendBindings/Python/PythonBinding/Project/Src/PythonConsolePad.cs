// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsolePad : ScriptingConsolePad
	{		
		protected override IScriptingConsoleHost CreateConsoleHost(IScriptingConsoleTextEditor consoleTextEditor, 
			IControlDispatcher dispatcher)
		{
			return new PythonConsoleHost(consoleTextEditor, dispatcher);
		}
		
		protected override string SyntaxHighlightingName {
			get { return "Python"; }
		}
	}
}
