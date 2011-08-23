// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Debugging;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Gui
{
	[TestFixture]
	public class DebugRubyCommandTests
	{
		MockDebugger debugger;
		RunDebugRubyCommand command;
		
		[SetUp]
		public void Init()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent(@"C:\Projects\test.rb");

			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			options.RubyFileName = @"C:\IronRuby\ir.exe";
		
			debugger = new MockDebugger();
			command = new RunDebugRubyCommand(workbench, options, debugger);
			command.Run();
		}
				
		[Test]
		public void Run_RubyFileOpen_DebuggerStartMethodCalled()
		{
			bool startMethodCalled = debugger.StartMethodCalled;
			Assert.IsTrue(startMethodCalled);
		}
		
		[Test]
		public void Run_RubyFileOpen_ProcessInfoFileNameContainsPathToIronRubyConsole()
		{
			string fileName = debugger.ProcessStartInfo.FileName;
			string expectedFileName = @"C:\IronRuby\ir.exe";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Run_RubyFileOpen_ProcessInfoArgsHasDebugArgument()
		{
			string arguments = debugger.ProcessStartInfo.Arguments;
			string expectedArguments = "--disable-gems -D \"test.rb\"";
			
			Assert.AreEqual(expectedArguments, arguments);
		}
	}
}
