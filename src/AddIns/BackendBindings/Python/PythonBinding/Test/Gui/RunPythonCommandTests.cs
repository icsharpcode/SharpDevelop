// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Gui
{
	/// <summary>
	/// Tests that the RunPythonCommand class runs the Python console
	/// passing the filename of the python script active in SharpDevelop.
	/// </summary>
	[TestFixture]
	public class RunPythonCommandTests
	{
		MockDebugger debugger;
		RunPythonCommand command;
		
		[SetUp]
		public void Init()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent(@"C:\Projects\test.py");

			Properties p = new Properties();
			PythonAddInOptions options = new PythonAddInOptions(p);
			options.PythonFileName = @"C:\IronPython\ipy.exe";
		
			debugger = new MockDebugger();
			command = new RunPythonCommand(workbench, options, debugger);
			command.Run();
		}
		
		[Test]
		public void BaseClass_NewInstance_IsAbstractCommand()
		{
			Assert.IsNotNull(command as AbstractCommand);
		}
		
		[Test]
		public void Run_PythonFileOpen_DebuggerStartWithoutDebuggingMethodCalled()
		{
			bool result = debugger.StartWithoutDebuggingMethodCalled;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Run_PythonFileOpen_CommandPromptExePassedToDebugger()
		{
			string fileName = debugger.ProcessStartInfo.FileName;
			string expectedFileName = "cmd.exe";
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Run_PythonFileOpen_IronPythonConsoleAndPythonFileNameAndPausePassedAsCommandLineArguments()
		{
			string args = debugger.ProcessStartInfo.Arguments;
			string expectedArgs = "/c \"\"C:\\IronPython\\ipy.exe\" \"test.py\"\" & pause";
			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void Run_PythonFileOpen_IronPythonConsoleWorkingDirectoryIsPathToPythonScriptFileBeingRun()
		{
			string workingDirectory = debugger.ProcessStartInfo.WorkingDirectory;
			string expectedWorkingDirectory = @"C:\Projects";
			Assert.AreEqual(expectedWorkingDirectory, workingDirectory);
		}
	}
}
