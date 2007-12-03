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
		MockProcessRunner processRunner;
		MessageViewCategory messageViewCategory;
		bool messageViewCategoryCleared;
		MockPadDescriptor padDescriptor;
		bool isRunning;
		
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
			padDescriptor = new MockPadDescriptor();
			
			// Create the Python binding addin options.
			Properties p = new Properties();
			AddInOptions options = new AddInOptions(p);
			options.PythonFileName = @"C:\IronPython\ipy.exe";
			
			// Create the process runner.
			processRunner = new MockProcessRunner();
			processRunner.OutputText = "Test\r\nOutput";
			
			// Create the message view category.
			messageViewCategory = new MessageViewCategory("Python");
			messageViewCategory.Cleared += MessageViewCategoryCleared;
	
			// Run the command.
			RunPythonCommand command = new RunPythonCommand(workbench, options, processRunner, messageViewCategory, padDescriptor);
			command.Run();
			
			// Check that the IsPythonRunning thinks the command is still
			// running.
			IsPythonRunningCondition condition = new IsPythonRunningCondition();
			isRunning = condition.IsValid(null, null);
			
			// The python console process exits.
			processRunner.RaiseExitEvent();
		}
		
		[Test]
		public void CommandLine()
		{
			// Check the correct filename was used to execute the command.
			Assert.AreEqual("C:\\IronPython\\ipy.exe \"C:\\Projects\\test.py\"", processRunner.CommandLine);
		}
		
		[Test]
		public void MessageViewCategoryClearTextCalled()
		{
			Assert.IsTrue(messageViewCategoryCleared);
		}
		
		/// <summary>
		/// Message view category should have the command line
		/// used to start the python console and any output from it.
		/// </summary>
		[Test]
		public void MessageViewCategoryText()
		{
			string expectedText = "Running Python...\r\n" +
				"C:\\IronPython\\ipy.exe \"C:\\Projects\\test.py\"\r\n" +
				"Test\r\n" +
				"Output\r\n";
			Assert.AreEqual(expectedText, messageViewCategory.Text);
		}
		
		[Test]
		public void OutputWindowPadBroughtToFront()
		{
			Assert.IsTrue(padDescriptor.BringPadToFrontCalled);
		}

		/// <summary>
		/// Until the ipy.exe process exits the RunPythonCommand
		/// should return true from the IsRunning property.
		/// </summary>
		[Test]
		public void IsPythonRunningBeforeExit()
		{
			Assert.IsTrue(isRunning);
		}
		
		[Test]
		public void IsPythonRunningAfterExit()
		{
			IsPythonRunningCondition condition = new IsPythonRunningCondition();
			Assert.IsFalse(condition.IsValid(null, null));
		}
		
		void MessageViewCategoryCleared(object sender, EventArgs e)
		{
			messageViewCategoryCleared = true;
		}
	}
}
