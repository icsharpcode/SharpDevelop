// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
