// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class TestableThreadSafeScriptingConsole : ThreadSafeScriptingConsole
	{
		public FakeScriptingConsole NonThreadSafeScriptingConsole;
		public FakeControlDispatcher Dispatcher;
		public TestableThreadSafeScriptingConsoleEvents ConsoleEvents;
		
		public TestableThreadSafeScriptingConsole()
			: this(new FakeScriptingConsole(), 
			       new TestableThreadSafeScriptingConsoleEvents(), 
			       new FakeControlDispatcher())
		{
		}
		
		TestableThreadSafeScriptingConsole(IScriptingConsole nonThreadSafeScriptingConsole,
			ThreadSafeScriptingConsoleEvents consoleEvents,		                                   
			IControlDispatcher dispatcher)
			: base(nonThreadSafeScriptingConsole, consoleEvents, dispatcher)
		{
			NonThreadSafeScriptingConsole = (FakeScriptingConsole)nonThreadSafeScriptingConsole;
			ConsoleEvents = (TestableThreadSafeScriptingConsoleEvents)consoleEvents;
			Dispatcher = (FakeControlDispatcher)dispatcher;
			
			ConsoleEvents.Dispatcher = Dispatcher;
			ConsoleEvents.NonThreadSafeScriptingConsole = NonThreadSafeScriptingConsole;
		}
	}
}
