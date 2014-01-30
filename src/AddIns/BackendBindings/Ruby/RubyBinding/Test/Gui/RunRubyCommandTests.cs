// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
