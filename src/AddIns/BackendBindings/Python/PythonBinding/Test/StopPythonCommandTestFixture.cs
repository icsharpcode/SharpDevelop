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
	/// Tests that the StopPythonCommand kills the python console process.
	/// </summary>
	[TestFixture]
	public class StopPythonCommandTestFixture
	{
		MockProcessRunner processRunner;
		
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
			
			// Create a dummy output window pad descriptor.
			MockPadDescriptor padDescriptor = new MockPadDescriptor();
			
			// Create the Python binding addin options.
			Properties p = new Properties();
			AddInOptions options = new AddInOptions(p);
			options.PythonFileName = @"C:\IronPython\ipy.exe";
			
			// Create the process runner.
			processRunner = new MockProcessRunner();
			
			// Create the message view category.
			MessageViewCategory messageViewCategory = new MessageViewCategory("Python");
	
			// Run the command.
			RunPythonCommand command = new RunPythonCommand(workbench, options, processRunner, messageViewCategory, padDescriptor);
			command.Run();
			
			StopPythonCommand stopCommand = new StopPythonCommand();
			stopCommand.Run();
		}
		
		[Test]
		public void IsStopped()
		{
			// Check that the IsPythonRunning thinks the command has stopped
			IsPythonRunningCondition condition = new IsPythonRunningCondition();
			Assert.IsFalse(condition.IsValid(null, null));
		}
		
		[Test]
		public void ProcessRunnerStopped()
		{
			Assert.IsTrue(processRunner.KillCalled);
		}
	}
}
