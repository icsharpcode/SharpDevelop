// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		
		TestableRubyConsole(IConsoleTextEditor consoleTextEditor, RubyCommandLine commandLine)
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
