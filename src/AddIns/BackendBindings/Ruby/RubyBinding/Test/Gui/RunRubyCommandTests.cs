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
		
		[SetUp]
		public void Init()
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
		public void Run_RubyFileOpen_RubyCommandIsAbstractCommand()
		{
			AbstractCommand abstractCommand = command as AbstractCommand;
			Assert.IsNotNull(abstractCommand);
		}
		
		[Test]
		public void Run_RubyFileOpen_DebuggerStartWithoutDebuggingMethodCalled()
		{
			bool startCalled = debugger.StartWithoutDebuggingMethodCalled;
			Assert.IsTrue(startCalled);
		}
		
		[Test]
		public void Run_RubyFileOpen_ProcessInfoFileNameIsIronRubyConsole()
		{
			string fileName = debugger.ProcessStartInfo.FileName;
			string expectedFileName = "cmd.exe";
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Run_RubyFileOpen_ProcessInfoArgsContainsFileNameActiveInTextEditor()
		{
			string arguments = debugger.ProcessStartInfo.Arguments;
			string expectedArguments = "/c \"\"C:\\IronRuby\\ir.exe\" --disable-gems \"test.rb\"\" & pause";
			
			Assert.AreEqual(expectedArguments, arguments);
		}
		
		[Test]
		public void Run_RubyFileOpen_WorkingDirectoryIsSameDirectoryAsFileBeingRun()
		{
			string directory = debugger.ProcessStartInfo.WorkingDirectory;
			string expectedDirectory = @"C:\Projects";
			Assert.AreEqual(expectedDirectory, directory);
		}
	}
}
