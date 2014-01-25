// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class TestableScriptingConsole : ScriptingConsole
	{
		public FakeConsoleTextEditor FakeConsoleTextEditor;
		public bool IsLineReceivedEventFired;
		public int UnreadLineCountWhenLineReceivedEventFired = -1;
		public ScriptingConsoleUnreadLines UnreadLines;
		
		public TestableScriptingConsole()
			: this(new FakeConsoleTextEditor(), new ScriptingConsoleUnreadLines())
		{
			MemberProvider = new MockMemberProvider();
		}
		
		TestableScriptingConsole(IScriptingConsoleTextEditor consoleTextEditor, ScriptingConsoleUnreadLines unreadLines)
			: base(consoleTextEditor, unreadLines)
		{
			FakeConsoleTextEditor = (FakeConsoleTextEditor)consoleTextEditor;
			UnreadLines = unreadLines;
		}
		
		public string[] GetUnreadLines()
		{
			return UnreadLines.ToArray();
		}
		
		protected override void FireLineReceivedEvent()
		{
			IsLineReceivedEventFired = true;
			UnreadLineCountWhenLineReceivedEventFired = UnreadLines.Count;
		}
		
		public void CallBaseFireLineReceivedEvent()
		{
			base.FireLineReceivedEvent();
		}
	}
}
