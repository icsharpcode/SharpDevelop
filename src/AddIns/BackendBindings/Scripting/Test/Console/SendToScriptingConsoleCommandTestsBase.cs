// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Console
{
	public abstract class SendToScriptingConsoleCommandTestsBase
	{
		protected MockConsoleTextEditor fakeConsoleTextEditor;
		protected MockTextEditor fakeTextEditor;
		protected MockWorkbench workbench;
		protected MockScriptingConsole fakeConsole;
		
		public void CreateFakeWorkbench()
		{
			workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.py");
			fakeConsoleTextEditor = workbench.MockScriptingConsolePad.MockConsoleTextEditor;
			fakeConsole = workbench.MockScriptingConsolePad.MockScriptingConsole;
			fakeTextEditor = workbench.ActiveMockEditableViewContent.MockTextEditor;
		}
	}
}
