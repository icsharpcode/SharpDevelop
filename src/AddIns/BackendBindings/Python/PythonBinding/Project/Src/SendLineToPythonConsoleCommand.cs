// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class SendLineToPythonConsoleCommand : SendLineToScriptingConsoleCommand
	{	
		public SendLineToPythonConsoleCommand()
			: base(new PythonWorkbench())
		{
		}
	}
}
