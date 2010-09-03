// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Hosting;

namespace RubyBinding.Tests.Utils
{
	public class TestableRubyConsole : RubyConsole
	{
		public MockConsoleTextEditor MockConsoleTextEditor;
		public FakeLock LockCreated;
		public bool IsLineReceivedEventFired;
		public int UnreadLineCountWhenLineReceivedEventFired = -1;
		
		public TestableRubyConsole()
			: this(new MockConsoleTextEditor(), new RubyCommandLine())
		{
		}
		
		TestableRubyConsole(IScriptingConsoleTextEditor consoleTextEditor, RubyCommandLine commandLine)
			: base(consoleTextEditor)
		{
			CommandLine = commandLine;
			MockConsoleTextEditor = (MockConsoleTextEditor)consoleTextEditor;
		}
		
		public List<string> GetUnreadLinesList()
		{
			return base.unreadLines;
		}
		
		public string[] GetUnreadLines()
		{
			return base.unreadLines.ToArray();
		}
		
		protected override ILock CreateLock(List<string> lines)
		{
			LockCreated = new FakeLock(lines);
			return LockCreated;
		}
		
		protected override void FireLineReceivedEvent()
		{
			IsLineReceivedEventFired = true;
			UnreadLineCountWhenLineReceivedEventFired = LockCreated.Lines.Count;
		}
	}
}
