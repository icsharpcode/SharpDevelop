// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	public class MockPythonConsolePad : IPythonConsolePad
	{
		public MockConsoleTextEditor MockConsoleTextEditor = new MockConsoleTextEditor();
		public MockPythonConsole MockPythonConsole = new MockPythonConsole();
		
		public bool BringToFrontCalled;
		
		public void BringToFront()
		{
			BringToFrontCalled = true;
		}
		
		public IConsoleTextEditor ConsoleTextEditor {
			get { return MockConsoleTextEditor; }
		}
		
		public IPythonConsole PythonConsole {
			get { return MockPythonConsole; }
		}
	}
}
