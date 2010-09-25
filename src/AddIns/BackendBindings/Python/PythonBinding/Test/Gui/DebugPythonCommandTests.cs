// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Debugging;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Gui
{
	[TestFixture]
	public class DebugPythonCommandTestFixture
	{
		MockDebugger debugger;
		RunDebugPythonCommand command;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent(@"C:\Projects\test.py");

			Properties p = new Properties();
			PythonAddInOptions options = new PythonAddInOptions(p);
			options.PythonFileName = @"C:\IronPython\ipy.exe";
		
			debugger = new MockDebugger();
			command = new RunDebugPythonCommand(workbench, options, debugger);
			command.Run();
		}
				
		[Test]
		public void Run_PythonFileOpen_DebuggerStartMethodCalled()
		{
			bool result = debugger.StartMethodCalled;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Run_PythonFileOpen_IronPythonConsoleFileNamePassedToDebugger()
		{
			string fileName = debugger.ProcessStartInfo.FileName;
			string expectedFileName = @"C:\IronPython\ipy.exe";
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Run_PythonFileOpen_DebugOptionsPassedToIronPythonConsole()
		{
			string args = debugger.ProcessStartInfo.Arguments;
			string expectedArgs = "-X:Debug \"test.py\"";
			Assert.AreEqual(expectedArgs, args);
		}
	}
}
