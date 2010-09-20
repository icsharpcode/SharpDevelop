// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Gui
{
	/// <summary>
	/// Tests that the RunRubyCommand class runs the Ruby console
	/// passing the filename of the Ruby script active in SharpDevelop.
	/// </summary>
	[TestFixture]
	public class RunRubyCommandTests
	{
		MockDebugger debugger;
		RunRubyCommand command;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent(@"C:\Projects\test.rb");

			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			options.RubyFileName = @"C:\IronRuby\ir.exe";
		
			debugger = new MockDebugger();
			command = new RunRubyCommand(workbench, options, debugger);
			command.Run();
		}
		
		[Test]
		public void RunRubyCommandIsAbstractCommand()
		{
			Assert.IsNotNull(command as AbstractCommand);
		}
		
		[Test]
		public void DebuggerStartWithoutDebuggingMethodCalled()
		{
			Assert.IsTrue(debugger.StartWithoutDebuggingMethodCalled);
		}
		
		[Test]
		public void ProcessInfoFileNameIsIronRubyConsole()
		{
			Assert.AreEqual(@"C:\IronRuby\ir.exe", debugger.ProcessStartInfo.FileName);
		}
		
		[Test]
		public void ProcessInfoArgsContainsFileNameActiveInTextEditor()
		{
			Assert.AreEqual("test.rb", debugger.ProcessStartInfo.Arguments);
		}
		
		[Test]
		public void WorkingDirectoryIsSameDirectoryAsFileBeingRun()
		{
			Assert.AreEqual(@"C:\Projects", debugger.ProcessStartInfo.WorkingDirectory);
		}
	}
}
