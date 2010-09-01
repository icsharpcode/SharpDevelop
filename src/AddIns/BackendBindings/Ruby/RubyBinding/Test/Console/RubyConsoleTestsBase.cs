// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	public class RubyConsoleTestsBase
	{
		public MockConsoleTextEditor MockConsoleTextEditor;
		public TestableRubyConsole TestableRubyConsole;
		
		public void CreateRubyConsole()
		{
			TestableRubyConsole = new TestableRubyConsole();
			MockConsoleTextEditor = TestableRubyConsole.MockConsoleTextEditor;
		}
	}
}
