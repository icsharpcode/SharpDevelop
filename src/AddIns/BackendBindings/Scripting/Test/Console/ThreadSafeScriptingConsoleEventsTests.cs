// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ThreadSafeScriptingConsoleEventsTests
	{
		ThreadSafeScriptingConsoleEvents consoleEvents;
		
		void CreateConsoleEvents()
		{
			consoleEvents = new ThreadSafeScriptingConsoleEvents();
		}
		
		[Test]
		public void WaitForLine_DisposedEventSetAndLineReceivedEventSet_ReturnsFalse()
		{
			CreateConsoleEvents();
			consoleEvents.SetLineReceivedEvent();
			consoleEvents.SetDisposedEvent();
			
			bool wait = consoleEvents.WaitForLine();
			
			Assert.IsFalse(wait);
		}
	}
}
