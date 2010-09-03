// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;

namespace RubyBinding.Tests.Utils
{
	public class MockRubyConsolePad : IRubyConsolePad
	{
		public MockConsoleTextEditor MockConsoleTextEditor = new MockConsoleTextEditor();
		public MockRubyConsole MockRubyConsole = new MockRubyConsole();
		
		public bool BringToFrontCalled;
		
		public void BringToFront()
		{
			BringToFrontCalled = true;
		}
		
		public IConsoleTextEditor ConsoleTextEditor {
			get { return MockConsoleTextEditor; }
		}
		
		public IRubyConsole RubyConsole {
			get { return MockRubyConsole; }
		}
	}
}
