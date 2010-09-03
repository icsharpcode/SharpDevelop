// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
