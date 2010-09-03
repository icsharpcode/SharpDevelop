// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
