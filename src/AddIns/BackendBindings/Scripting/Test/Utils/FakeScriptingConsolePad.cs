// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeScriptingConsolePad : IScriptingConsolePad
	{
		public FakeConsoleTextEditor FakeConsoleTextEditor = new FakeConsoleTextEditor();
		public FakeScriptingConsole FakeScriptingConsole = new FakeScriptingConsole();
		
		public bool BringToFrontCalled;
		
		public void BringToFront()
		{
			BringToFrontCalled = true;
		}
		
		public IScriptingConsoleTextEditor ScriptingConsoleTextEditor {
			get { return FakeConsoleTextEditor; }
		}
		
		public IScriptingConsole ScriptingConsole {
			get { return FakeScriptingConsole; }
		}
	}
}
