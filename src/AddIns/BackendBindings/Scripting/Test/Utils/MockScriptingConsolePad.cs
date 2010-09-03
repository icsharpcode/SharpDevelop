// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockScriptingConsolePad : IScriptingConsolePad
	{
		public MockConsoleTextEditor MockConsoleTextEditor = new MockConsoleTextEditor();
		public MockScriptingConsole MockScriptingConsole = new MockScriptingConsole();
		
		public bool BringToFrontCalled;
		
		public void BringToFront()
		{
			BringToFrontCalled = true;
		}
		
		public IScriptingConsoleTextEditor ScriptingConsoleTextEditor {
			get { return MockConsoleTextEditor; }
		}
		
		public IScriptingConsole ScriptingConsole {
			get { return MockScriptingConsole; }
		}
	}
}
