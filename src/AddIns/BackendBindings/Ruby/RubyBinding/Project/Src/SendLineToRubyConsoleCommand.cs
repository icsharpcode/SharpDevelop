// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public class SendLineToRubyConsoleCommand : SendLineToScriptingConsoleCommand
	{		
		public SendLineToRubyConsoleCommand()
			: base(new RubyWorkbench())
		{
		}
	}
}
