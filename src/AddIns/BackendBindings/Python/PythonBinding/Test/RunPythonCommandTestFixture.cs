// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests that the RunPythonCommand class runs the Python console
	/// passing the filename of the python script active in SharpDevelop.
	/// </summary>
	[TestFixture]
	public class RunPythonCommandTestFixture
	{
		MockDebugger debugger;
		RunPythonCommand command;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			// Create dummy view content with the Python script.
			MockViewContent viewContent = new MockViewContent();
			viewContent.PrimaryFileName = @"C:\Projects\test.py";
			MockWorkbenchWindow workbenchWindow = new MockWorkbenchWindow();
			workbenchWindow.ActiveViewContent = viewContent;
			MockWorkbench workbench = new MockWorkbench();
			workbench.ActiveWorkbenchWindow = workbenchWindow;

			// Create the Python binding addin options.
			Properties p = new Properties();
			AddInOptions options = new AddInOptions(p);
			options.PythonFileName = @"C:\IronPython\ipy.exe";
		
			debugger = new MockDebugger();
			command = new RunPythonCommand(workbench, options, debugger);
			command.Run();
		}
		
		[Test]
		public void RunPythonCommandIsAbstractCommand()
		{
			Assert.IsNotNull(command as AbstractCommand);
		}
		
		[Test]
		public void DebuggerStartWithoutDebuggingMethodCalled()
		{
			Assert.IsTrue(debugger.StartWithoutDebuggingMethodCalled);
		}
		
		[Test]
		public void ProcessInfoFileName()
		{
			Assert.AreEqual(@"C:\IronPython\ipy.exe", debugger.ProcessStartInfo.FileName);
		}
		
		[Test]
		public void ProcessInfoArgs()
		{
			Assert.AreEqual("\"C:\\Projects\\test.py\"", debugger.ProcessStartInfo.Arguments);
		}
	}
}
