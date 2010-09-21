// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Console
{
	public abstract class SendToScriptingConsoleCommandTestsBase
	{
		protected FakeConsoleTextEditor fakeConsoleTextEditor;
		protected MockTextEditor fakeTextEditor;
		protected MockWorkbench workbench;
		protected FakeScriptingConsole fakeConsole;
		
		public void CreateFakeWorkbench()
		{
			workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.py");
			fakeConsoleTextEditor = workbench.FakeScriptingConsolePad.FakeConsoleTextEditor;
			fakeConsole = workbench.FakeScriptingConsolePad.FakeScriptingConsole;
			fakeTextEditor = workbench.ActiveMockEditableViewContent.MockTextEditor;
		}
	}
}
