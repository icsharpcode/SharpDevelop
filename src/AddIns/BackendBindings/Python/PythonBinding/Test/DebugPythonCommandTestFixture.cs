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

namespace PythonBinding.Tests
{
	[TestFixture]
	public class DebugPythonCommandTestFixture
	{
		MockDebugger debugger;
		RunDebugPythonCommand command;
		
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
			command = new RunDebugPythonCommand(workbench, options, debugger);
			command.Run();
		}
				
		[Test]
		public void DebuggerStartMethodCalled()
		{
			Assert.IsTrue(debugger.StartMethodCalled);
		}
		
		[Test]
		public void ProcessInfoFileName()
		{
			Assert.AreEqual(@"C:\IronPython\ipy.exe", debugger.ProcessStartInfo.FileName);
		}
		
		[Test]
		public void ProcessInfoArgs()
		{
			Assert.AreEqual("-D \"C:\\Projects\\test.py\"", debugger.ProcessStartInfo.Arguments);
		}
	}
}
