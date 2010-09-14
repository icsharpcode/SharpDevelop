// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsolePad : ScriptingConsolePad
	{		
		protected override IScriptingConsoleHost CreateConsoleHost(IScriptingConsoleTextEditor consoleTextEditor, 
			IControlDispatcher dispatcher)
		{
			return new RubyConsoleHost(consoleTextEditor, dispatcher);
		}
		
		protected override string SyntaxHighlightingName {
			get { return "Ruby"; }
		}
	}
}
