// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
