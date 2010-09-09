// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Console
{
	public class ScriptingConsoleTestsBase
	{
		public MockConsoleTextEditor MockConsoleTextEditor;
		public TestableScriptingConsole TestableScriptingConsole;
		
		public void CreateConsole()
		{
			TestableScriptingConsole = new TestableScriptingConsole();
			MockConsoleTextEditor = TestableScriptingConsole.MockConsoleTextEditor;
		}
		
		public void WritePrompt()
		{
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
		}
	}
}
