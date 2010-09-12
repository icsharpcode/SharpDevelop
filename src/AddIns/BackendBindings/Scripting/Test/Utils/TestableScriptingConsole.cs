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
		
		public TestableScriptingConsole()
			: this(new FakeConsoleTextEditor())
		{
			MemberProvider = new MockMemberProvider();
		}
		
		TestableScriptingConsole(IScriptingConsoleTextEditor consoleTextEditor)
			: base(consoleTextEditor)
		{
			FakeConsoleTextEditor = (FakeConsoleTextEditor)consoleTextEditor;
		}
		
		public List<string> GetUnreadLinesList()
		{
			return base.unreadLines;
		}
		
		public string[] GetUnreadLines()
		{
			return base.unreadLines.ToArray();
		}
		
		protected override void FireLineReceivedEvent()
		{
			IsLineReceivedEventFired = true;
			UnreadLineCountWhenLineReceivedEventFired = base.unreadLines.Count;
		}
		
		public void CallBaseFireLineReceivedEvent()
		{
			base.FireLineReceivedEvent();
		}
	}
}
