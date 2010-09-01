// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.PythonBinding;
using PythonBinding.Tests.Utils;
using NUnit.Framework;

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
			string expectedArgs = "-X:Debug \"C:\\Projects\\test.py\"";
			Assert.AreEqual(expectedArgs, args);
		}
	}
}
